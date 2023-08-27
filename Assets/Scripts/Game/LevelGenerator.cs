using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct HallwayReplacementTiles
    {
        public Vector3Int direction;
        public List<Tile> replacementTiles; // each element = each layer of tiles
    }

    private const int MAX_ATTEMPTS = 99;

    [SerializeField] private int m_roomsToGenerate = 5;
    [SerializeField] private Tile m_floorTile;
    [SerializeField] private Tile m_roomLinkTile;
    [SerializeField] private Tilemap m_dungeonMap;
    [SerializeField] private Tilemap m_startRoom;

    [SerializeField] private List<Tilemap> m_rooms = new List<Tilemap>();
    [SerializeField] private List<HallwayReplacementTiles> m_hallwayReplacementTiles = new List<HallwayReplacementTiles>();

    private int m_roomsCount;
    private int m_attemptsCounter;
    private List<Vector3Int> m_roomLinks = new List<Vector3Int>();

    [Button]
    public void Generate()
    {
        m_roomLinks.Clear();
        m_dungeonMap.ClearAllTiles();
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


        Tilemap lastRoom = m_dungeonMap;
        for (m_roomsCount = 0; m_roomsCount < m_roomsToGenerate;)
        {
            AddRoomToDungeon();
        }

        RemoveUnusedHallways();
        ReplaceRoomLinkTiles();
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

        List<Vector3Int> roomLinkTiles = GetRoomLinkTiles(m_dungeonMap, m_roomLinkTile);

        List<Tilemap> tilemapsCopy = new List<Tilemap>(m_rooms);

        while (tilemapsCopy.Count > 0)
        {
            Tilemap newRoom = tilemapsCopy.GetRandomElementAndRemove();

            while (roomLinkTiles.Count > 0)
            {
                // random door from room 1
                Vector3Int randDoor = roomLinkTiles.GetRandomElementAndRemove();

                // check all 4 directions for a space
                foreach (var dir in checkDirections)
                {
                    Vector3Int cellToCheck = dir + randDoor;
                    TileBase tile = m_dungeonMap.GetTile<TileBase>(cellToCheck);
                    if (tile == null)
                    {
                        Vector3Int validCell = cellToCheck;

                        List<Vector3Int> doorsInNewRoom = GetRoomLinkTiles(newRoom, m_roomLinkTile);

                        foreach (var doorInNewRoom in doorsInNewRoom)
                        {
                            Vector3Int offset = validCell - doorInNewRoom;
                            bool hasOverlap = CheckOverlap(m_dungeonMap, newRoom, offset);
                            if (!hasOverlap)
                            {
                                CopyTilemap(newRoom, m_dungeonMap, offset);
                                m_roomLinks.Add(randDoor);
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

        Debug.Log("No more valid positions found");
    }

    private void ValidateSettings()
    {
        // check variables are set
    }

    private void AddStartRoom()
    {
        CopyTilemap(m_startRoom, m_dungeonMap, Vector3Int.zero);
    }

    private List<Vector3Int> GetRoomLinkTiles(Tilemap tilemap, Tile roomLinkTile)
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
        var tm1Tiles = m_dungeonMap.cellBounds.allPositionsWithin;
        int count = 0;
        foreach (var item in tm1Tiles)
        {
            if (m_dungeonMap.GetTile(item) != null)
            {
                count++;
            }
        }
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

    private void ReplaceRoomLinkTiles()
    {
        foreach (var cell in m_roomLinks)
        {
            if (m_dungeonMap.GetTile(cell) == m_roomLinkTile)
            {
                m_dungeonMap.SetTile(cell, m_floorTile);
            }
        }
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

        for (int i = 0; i < m_roomLinks.Count; i++)
        {
            Vector3Int cell = m_roomLinks[i];
            for (int c = 0; c < m_hallwayReplacementTiles.Count; c++)
            {
                Vector3Int dir = m_hallwayReplacementTiles[c].direction;
                Vector3Int checkCell = cell + dir;
                if (m_dungeonMap.GetTile(checkCell) == null)
                {
                    // check adjacent tiles
                    Vector3Int adjacentDir1 = m_hallwayReplacementTiles[(c + 1) % m_hallwayReplacementTiles.Count].direction;
                    Vector3Int adjacentDir2 = m_hallwayReplacementTiles[(c + 3) % m_hallwayReplacementTiles.Count].direction;

                    Vector3Int adjCell1 = cell + adjacentDir1;
                    Vector3Int adjCell2 = cell + adjacentDir2;

                    // tile is empty so remove cell and cell + 1
                    m_dungeonMap.SetTile(cell, m_hallwayReplacementTiles[c].replacementTiles[0]);
                    m_dungeonMap.SetTile(adjCell1, m_hallwayReplacementTiles[c].replacementTiles[0]);
                    m_dungeonMap.SetTile(adjCell2, m_hallwayReplacementTiles[c].replacementTiles[0]);

                    int layersToRemove = 2;
                    for (int j = 0; j < layersToRemove; j++)
                    {
                        // remove next layers
                        cell += dir * -1;
                        adjCell1 = cell + adjacentDir1;
                        adjCell2 = cell + adjacentDir2;

                        m_dungeonMap.SetTile(cell, m_hallwayReplacementTiles[c].replacementTiles[j + 1]);
                        m_dungeonMap.SetTile(adjCell1, m_hallwayReplacementTiles[c].replacementTiles[j + 1]);
                        m_dungeonMap.SetTile(adjCell2, m_hallwayReplacementTiles[c].replacementTiles[j + 1]);
                    }

                    removedRoomLinks.Add(cell);
                    break;
                }
            }
        }

        foreach (var cell in removedRoomLinks)
        {
            m_roomLinks.Remove(cell);
        }
    }

    private void CopyTilemap(Tilemap from, Tilemap to, Vector3Int tileOffset)
    {
        var tilesInBounds = from.cellBounds.allPositionsWithin;
        foreach (var vec3i in tilesInBounds)
        {
            TileBase tile = from.GetTile<TileBase>(vec3i);
            if (tile)
            {
                Vector3Int newTilePos = vec3i + tileOffset;
                if (tile == m_roomLinkTile)
                {
                    m_roomLinks.Add(newTilePos);
                }
                to.SetTile(newTilePos, tile);
            }
        }
    }
}
