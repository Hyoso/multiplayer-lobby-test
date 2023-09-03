using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class LevelGenerator : NetworkBehaviour
{
    [System.Serializable]
    public struct HallwayReplacementTiles
    {
        public Vector3Int direction;
        public List<Tile> replacementTiles; // each element = each layer of tiles
    }

    [System.Serializable]
    public class Room
    {
        public int roomId = -1;
        public Bounds bounds;
        public GameObject colliderGameObject;
        public RoomData roomData;
        public List<DoorTile> doorTiles = new List<DoorTile>();
        //public List<RoomEntranceTile> entranceTiles = new List<RoomEntranceTile>();
    }

    private const int MAX_ATTEMPTS = 99;

    [SerializeField] private NetworkVariable<int> m_seed = new NetworkVariable<int>();
    [SerializeField] private Vector3Int m_spawnOffset;
    [SerializeField] private int m_roomsToGenerate = 5;
    [SerializeField] private Bounds m_mapBounds;
    [SerializeField] private Grid m_grid;
    [SerializeField] private DungeonMap m_dungeonMapObject;

    [SerializeField, ReadOnly] private Tilemap m_dungeonTilemap;
    [SerializeField, ReadOnly] private DungeonMap m_generatedDungeonMap;
    [SerializeField, ReadOnly] private GameObject m_currentDungeonMap;

    private int m_roomsCount;
    private int m_attemptsCounter;

    private void Start()
    {
    }

    public override async void OnNetworkSpawn()
    {
        SetupNetworkSettings();

        Random.InitState(m_seed.Value);
        await Generate();

        Debug.Log("Seed: " + m_seed.Value);
    }

    public async Task Generate()
    {
        if (m_currentDungeonMap)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_currentDungeonMap);
#endif
        }

        GameObject newDungeonMap = Instantiate(m_dungeonMapObject.gameObject);
        newDungeonMap.transform.parent = m_grid.transform;
        newDungeonMap.transform.localPosition = Vector3.zero;
        m_generatedDungeonMap = newDungeonMap.GetComponent<DungeonMap>();
        m_currentDungeonMap = newDungeonMap;
        m_dungeonTilemap = m_generatedDungeonMap.tilemap;

        AddStartRoom();

        /// #### ALGORITHM ### ///
        /// 
        // check start room for available doors (room links)
        // add to list
        // choose one at random
        // choose a room at random
        // check chosen room for available doors
        // add to list
        // check which doors can fit with the start room door without overlapping
        // additional: add bounds for the map
        ///
        /// #### ALGORITHM ### ///

        for (m_roomsCount = 0; m_roomsCount < m_roomsToGenerate;)
        {
            AddRoomToDungeon();
        }

        RemoveUnusedHallways();
        ReplaceRoomLinkTiles();
        CalculateRoomBounds();
        SetupRoomEntranceTiles();
        AddRoomDoors();
        // replace room entrance tiles
        ReplaceTiles(m_dungeonTilemap, m_generatedDungeonMap.roomEntranceCells, m_generatedDungeonMap.floorTile);
        await Task.Delay(50);
    }


    private void OnDrawGizmos()
    {
        GizmosUtils.DrawBoundingBox(m_mapBounds, Color.red);

        if (m_generatedDungeonMap != null && m_generatedDungeonMap.generatedRooms != null)
        {
            foreach (var room in m_generatedDungeonMap.generatedRooms)
            {
                GizmosUtils.DrawBoundingBox(room.bounds, Color.red);
            }
        }
    }

    [Button]
    private void AddRoomToDungeon()
    {
        List<Vector3Int> checkDirections = new List<Vector3Int>{
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        List<Vector3Int> roomLinkTiles = GetRoomLinkTiles(m_dungeonTilemap, m_generatedDungeonMap.roomLinkTile);

        List<RoomData> tilemapsCopy = new List<RoomData>(m_generatedDungeonMap.roomTilemaps);

        while (tilemapsCopy.Count > 0)
        {
            RoomData newRoomTilemap = tilemapsCopy.GetRandomElementAndRemove();
            Tilemap newRoom = newRoomTilemap.tilemap;

            while (roomLinkTiles.Count > 0)
            {
                // random door from room 1
                Vector3Int randDoor = roomLinkTiles.GetRandomElementAndRemove();

                // check all 4 directions for a space
                foreach (var dir in checkDirections)
                {
                    Vector3Int cellToCheck = dir + randDoor;
                    TileBase tile = m_dungeonTilemap.GetTile<TileBase>(cellToCheck);
                    if (tile == null)
                    {
                        Vector3Int validCell = cellToCheck;

                        List<Vector3Int> doorsInNewRoom = GetRoomLinkTiles(newRoom, m_generatedDungeonMap.roomLinkTile);

                        foreach (var doorInNewRoom in doorsInNewRoom)
                        {
                            Vector3Int offset = validCell - doorInNewRoom;
                            bool hasOverlap = CheckOverlap(m_dungeonTilemap, newRoom, offset);
                            bool withinBounds = CheckWithinBounds(newRoom, offset);
                            if (!hasOverlap && withinBounds)
                            {
                                CopyTilemap(newRoom, m_dungeonTilemap, offset);
                                GameObject colliderGO = CreateRoomCollider(newRoom, offset);
                                m_generatedDungeonMap.generatedRooms.Add(new Room()
                                {
                                    roomId = m_generatedDungeonMap.generatedRooms.Count,
                                    colliderGameObject = colliderGO,
                                    roomData = newRoomTilemap
                                });
                                m_generatedDungeonMap.roomLinks.Add(randDoor);

                                m_roomsCount++;
                                m_attemptsCounter = 0;
                                return;
                            }
                        }
                    }
                }
            }
        }

        m_attemptsCounter++;

        if (m_attemptsCounter > MAX_ATTEMPTS)
        {
            m_roomsCount = m_roomsToGenerate;
        }
    }

    private void AddStartRoom()
    {
        CopyTilemap(m_generatedDungeonMap.startRoom.tilemap, m_dungeonTilemap, m_spawnOffset);
        GameObject colliderGO = CreateRoomCollider(m_generatedDungeonMap.startRoom.tilemap, m_spawnOffset);
        m_generatedDungeonMap.generatedRooms.Add(new Room()
        {
            roomId = m_generatedDungeonMap.generatedRooms.Count,
            colliderGameObject = colliderGO,
            roomData = m_generatedDungeonMap.startRoom
        });
    }

    private List<Vector3Int> GetTileOfType(Tilemap tilemap, TileBase roomLinkTile)
    {
        int count = 0;
        var tilesInBounds = tilemap.cellBounds.allPositionsWithin;
        List<Vector3Int> tiles = new List<Vector3Int>();

        foreach (var vec3i in tilesInBounds)
        {
            TileBase tile = tilemap.GetTile<TileBase>(vec3i);
            if (tile != null)
            {
                count++;
            }
            if (tile == roomLinkTile)
            {
                tiles.Add(vec3i);
            }
        }


        return tiles;
    }

    private List<Vector3Int> GetRoomLinkTiles(Tilemap tilemap, TileBase roomLinkTile)
    {
        int count = 0;
        var tilesInBounds = tilemap.cellBounds.allPositionsWithin;
        List<Vector3Int> roomLinkTiles = new List<Vector3Int>();

        foreach (var vec3i in tilesInBounds)
        {
            TileBase tile = tilemap.GetTile<TileBase>(vec3i);
            if (tile != null)
            {
                count++;
            }
            if (tile == roomLinkTile)
            {
                roomLinkTiles.Add(vec3i);
            }
        }


        return roomLinkTiles;
    }

    public void PrintDungeonMapTileCount()
    {
        var tm1Tiles = m_dungeonTilemap.cellBounds.allPositionsWithin;
        int count = 0;
        foreach (var item in tm1Tiles)
        {
            if (m_dungeonTilemap.GetTile(item) != null)
            {
                count++;
            }
        }
    }

    private bool CheckWithinBounds(Tilemap tm1, Vector3Int offset)
    {
        var tilesInBounds = tm1.cellBounds.allPositionsWithin;
        foreach (var vec3i in tilesInBounds)
        {
            TileBase tile = tm1.GetTile(vec3i);
            if (tile)
            {
                Vector3Int newTilePos = vec3i + offset;
                if (!m_mapBounds.Contains(newTilePos))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool CheckOverlap(Tilemap tm1, Tilemap tm2, Vector3Int offset)
    {
        var tm1Tiles = tm1.cellBounds.allPositionsWithin;

        foreach (var tm1CellPos in tm1Tiles)
        {
            bool containsValidTile = tm1.GetTile(tm1CellPos);
            if (containsValidTile)
            {
                Vector3Int checkCell = tm1CellPos - offset;
                TileBase tm2Tile = tm2.GetTile(checkCell);
                if (tm2Tile)
                {
                    // tm2Tile has a tile so there's an overlap at this cell
                    return true;
                }
            }
        }

        return false;
    }

    private void ReplaceTiles(Tilemap tm, List<Vector3Int> cellsToReplace, TileBase tile)
    {
        foreach (var cell in cellsToReplace)
        {
            tm.SetTile(cell, tile);
        }
    }

    private void ReplaceRoomLinkTiles()
    {
        foreach (var cell in m_generatedDungeonMap.roomLinks)
        {
            if (m_dungeonTilemap.GetTile(cell) == m_generatedDungeonMap.roomLinkTile)
            {
                m_dungeonTilemap.SetTile(cell, m_generatedDungeonMap.floorTile);
            }
        }
    }

    private void AddRoomDoors()
    {
        List<Vector3Int> checkDirections = new List<Vector3Int>{
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        List<Vector3Int> doorCells = GetTileOfType(m_dungeonTilemap, m_generatedDungeonMap.doorTile);

        Dictionary<Vector3Int, DoorTile> doorsDict = new Dictionary<Vector3Int, DoorTile>();

        foreach (var cell in doorCells)
        {
            DoorTile door = GameObject.Instantiate(m_generatedDungeonMap.doorPrefab).GetComponent<DoorTile>();
            Vector3 cellWorldPos = cell;
            door.transform.position = cellWorldPos;
            door.transform.parent = m_generatedDungeonMap.doorsParent;
            doorsDict.Add(cell, door);

            for (int i = 0; i < checkDirections.Count; i++)
            {
                Vector3Int checkCell = checkDirections[i] + cell;
                if (m_dungeonTilemap.GetTile(checkCell) == m_generatedDungeonMap.roomEntranceTile)
                {
                    // there's a room entrance tile in this direction
                    // so the direction of this door is in the opposite direction
                    // e.g. the room entrance is south of this tile, so this is a north facing door

                    int convertedDirection = ((i + 2) % checkDirections.Count) + 1;
                    door.direction = (DoorTile.Direction)convertedDirection;
                }
            }
        }

        foreach (var cell in doorCells)
        {
            for (int i = 0; i < m_generatedDungeonMap.colliderTilemaps.Count; i++)
            {
                Tilemap map = m_generatedDungeonMap.colliderTilemaps[i];
                TileBase tile = map.GetTile(cell);
                if (tile != null)
                {
                    m_generatedDungeonMap.generatedRooms[i].doorTiles.Add(doorsDict[cell]);
                    break;
                }
            }
        }

        doorsDict.Clear();

        ReplaceTiles(m_dungeonTilemap, doorCells, m_generatedDungeonMap.floorTile);
    }

    private void RemoveUnusedHallways()
    {
        /// #### ALGORITHM ### ///
        /// 
        // loop through room links list
        // find if north/east/south/west tile is empty
        // if empty then back track in opposite direction
        // remove current tile and cur tile + 1, also remove adjacent tiles
        // for cur tile + 2, replace with wall tile
        /// 
        /// #### ALGORITHM ### ///
        /// 
        List<Vector3Int> removedRoomLinks = new List<Vector3Int>();

        for (int i = 0; i < m_generatedDungeonMap.roomLinks.Count; i++)
        {
            Vector3Int cell = m_generatedDungeonMap.roomLinks[i];
            for (int c = 0; c < m_generatedDungeonMap.hallwayReplacementTiles.Count; c++)
            {
                Vector3Int dir = m_generatedDungeonMap.hallwayReplacementTiles[c].direction;
                Vector3Int checkCell = cell + dir;
                if (m_dungeonTilemap.GetTile(checkCell) == null)
                {
                    // check adjacent tiles
                    Vector3Int adjacentDir1 = m_generatedDungeonMap.hallwayReplacementTiles[(c + 1) % m_generatedDungeonMap.hallwayReplacementTiles.Count].direction;
                    Vector3Int adjacentDir2 = m_generatedDungeonMap.hallwayReplacementTiles[(c + 3) % m_generatedDungeonMap.hallwayReplacementTiles.Count].direction;

                    Vector3Int adjCell1 = cell + adjacentDir1;
                    Vector3Int adjCell2 = cell + adjacentDir2;

                    // tile is empty so remove cell and cell + 1
                    m_dungeonTilemap.SetTile(cell, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[0]);
                    m_dungeonTilemap.SetTile(adjCell1, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[0]);
                    m_dungeonTilemap.SetTile(adjCell2, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[0]);

                    int layersToRemove = 2;
                    for (int j = 0; j < layersToRemove; j++)
                    {
                        // remove next layers
                        cell += dir * -1;
                        adjCell1 = cell + adjacentDir1;
                        adjCell2 = cell + adjacentDir2;

                        m_dungeonTilemap.SetTile(cell, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[j + 1]);
                        m_dungeonTilemap.SetTile(adjCell1, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[j + 1]);
                        m_dungeonTilemap.SetTile(adjCell2, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[j + 1]);
                    }

                    removedRoomLinks.Add(cell);
                    break;
                }
            }
        }

        foreach (var cell in removedRoomLinks)
        {
            m_generatedDungeonMap.roomLinks.Remove(cell);
        }
    }

    private GameObject CreateRoomCollider(Tilemap roomTM, Vector3Int roomOffset)
    {
        GameObject tilemapGameObject = Instantiate(m_generatedDungeonMap.tilemapColliderPrefab);
        tilemapGameObject.transform.parent = m_generatedDungeonMap.collidersParent;
        Tilemap colliderTilemap = tilemapGameObject.GetComponent<Tilemap>();
        m_generatedDungeonMap.colliderTilemaps.Add(colliderTilemap);
        CopyTilemap(roomTM, colliderTilemap, roomOffset, false);

        List<Vector3Int> doorLinkCells = GetRoomLinkTiles(colliderTilemap, m_generatedDungeonMap.roomLinkTile);

        for (int i = 0; i < doorLinkCells.Count; i++)
        {
            Vector3Int cell = doorLinkCells[i];
            for (int c = 0; c < m_generatedDungeonMap.hallwayReplacementTiles.Count; c++)
            {
                Vector3Int dir = m_generatedDungeonMap.hallwayReplacementTiles[c].direction;
                Vector3Int checkCell = cell + dir;
                if (colliderTilemap.GetTile(checkCell) == null)
                {
                    // check adjacent tiles
                    Vector3Int adjacentDir1 = m_generatedDungeonMap.hallwayReplacementTiles[(c + 1) % m_generatedDungeonMap.hallwayReplacementTiles.Count].direction;
                    Vector3Int adjacentDir2 = m_generatedDungeonMap.hallwayReplacementTiles[(c + 3) % m_generatedDungeonMap.hallwayReplacementTiles.Count].direction;

                    Vector3Int adjCell1 = cell + adjacentDir1;
                    Vector3Int adjCell2 = cell + adjacentDir2;

                    // tile is empty so remove cell and cell + 1
                    colliderTilemap.SetTile(cell, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[0]);
                    colliderTilemap.SetTile(adjCell1, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[0]);
                    colliderTilemap.SetTile(adjCell2, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[0]);

                    int layersToRemove = 2;
                    for (int j = 0; j < layersToRemove; j++)
                    {
                        // remove next layers
                        cell += dir * -1;
                        adjCell1 = cell + adjacentDir1;
                        adjCell2 = cell + adjacentDir2;

                        colliderTilemap.SetTile(cell, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[j + 1]);
                        colliderTilemap.SetTile(adjCell1, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[j + 1]);
                        colliderTilemap.SetTile(adjCell2, m_generatedDungeonMap.hallwayReplacementTiles[c].replacementTiles[j + 1]);
                    }

                    break;
                }
            }
        }

        return tilemapGameObject;
    }

    private void CalculateRoomBounds()
    {
        foreach (var room in m_generatedDungeonMap.generatedRooms)
        {
            TilemapCollider2D collider = room.colliderGameObject.GetComponent<TilemapCollider2D>();
            room.bounds = collider.bounds;
        }
    }

    private void SetupRoomEntranceTiles()
    {
        List<Vector3Int> checkDirections = new List<Vector3Int>{
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        var cells = m_dungeonTilemap.cellBounds.allPositionsWithin;
        foreach (var cell in cells)
        {
            TileBase tile = m_dungeonTilemap.GetTile<TileBase>(cell);
            if (tile == m_generatedDungeonMap.roomEntranceTile)
            {
                // check nearby cells, if they are door cells, then create trigger
                foreach (var dir in checkDirections)
                {
                    Vector3Int checkCell = dir + cell;
                    if (m_dungeonTilemap.GetTile(checkCell) == m_generatedDungeonMap.doorTile)
                    {
                        GameObject go = Instantiate(m_generatedDungeonMap.roomEntrancePrefab);
                        go.transform.position = cell;
                        go.transform.parent = m_generatedDungeonMap.roomEntranceTilesParent;
                    }
                }

                m_generatedDungeonMap.roomEntranceCells.Add(cell);
            }
        }
    }

    private void CopyTilemap(Tilemap from, Tilemap to, Vector3Int tileOffset, bool addToRoomLinks = true)
    {
        var tilesInBounds = from.cellBounds.allPositionsWithin;
        foreach (var vec3i in tilesInBounds)
        {
            TileBase tile = from.GetTile<TileBase>(vec3i);
            if (tile)
            {
                Vector3Int newTilePos = vec3i + tileOffset;
                if (tile == m_generatedDungeonMap.roomLinkTile && addToRoomLinks)
                {
                    m_generatedDungeonMap.roomLinks.Add(newTilePos);
                }
                to.SetTile(newTilePos, tile);
            }
        }
    }
     
    private void SetupNetworkSettings()
    {
        if (IsServer && IsClient)
        {
            m_seed.Value = System.Environment.TickCount;
        }
    }
}
