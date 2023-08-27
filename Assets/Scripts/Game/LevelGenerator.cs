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
    private const int MAX_ATTEMPTS = 99;

    [SerializeField] private int m_roomsToGenerate = 5;

    public Tilemap m_dungeonMap;
    public Tilemap m_startRoom;
    public List<Tilemap> m_rooms = new List<Tilemap>();
    public Tile m_doorTile;
    public Tile m_floorTile;

    private int m_roomsCount;
    private int m_attemptsCounter;

    [Button]
    public void Generate()
    {
        m_dungeonMap.ClearAllTiles();
        AddStartRoom();

        Tilemap lastRoom = m_dungeonMap;
        for (m_roomsCount = 0; m_roomsCount < m_roomsToGenerate;)
        {
            AddRoomToDungeon();
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

        List<Vector3Int> doorTiles = GetDoorTiles(m_dungeonMap, m_doorTile);

        List<Tilemap> tilemapsCopy = new List<Tilemap>(m_rooms);

        while (tilemapsCopy.Count > 0)
        {
            Tilemap newRoom = tilemapsCopy.GetRandomElementAndRemove();

            while (doorTiles.Count > 0)
            {
                // random door from room 1
                Vector3Int randDoor = doorTiles.GetRandomElementAndRemove();

                // check all 4 directions for a space
                foreach (var dir in checkDirections)
                {
                    Vector3Int cellToCheck = dir + randDoor;
                    TileBase tile = m_dungeonMap.GetTile<TileBase>(cellToCheck);
                    if (tile == null)
                    {
                        Vector3Int validCell = cellToCheck;

                        // todo: change m_Startroom to randomly selected one from list later

                        List<Vector3Int> doorsInNewRoom = GetDoorTiles(newRoom, m_doorTile);

                        foreach (var doorInNewRoom in doorsInNewRoom)
                        {
                            Vector3Int offset = validCell - doorInNewRoom;
                            bool hasOverlap = CheckOverlap(m_dungeonMap, newRoom, offset);
                            if (!hasOverlap)
                            {
                                CopyTilemap(newRoom, m_dungeonMap, offset);
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

    [Button]
    private void AddStartRoom()
    {
        CopyTilemap(m_startRoom, m_dungeonMap, Vector3Int.zero);
    }

    private List<Vector3Int> GetDoorTiles(Tilemap tilemap, Tile doorTile)
    {
        int count = 0;
        var tilesInBounds = tilemap.cellBounds.allPositionsWithin;
        List<Vector3Int> doorTiles = new List<Vector3Int>();

        foreach (var vec3i in tilesInBounds)
        {
            TileBase tile = tilemap.GetTile<TileBase>(vec3i);
            if (tile != null)
            {
                count++;
            }
            if (tile == doorTile)
            {
                doorTiles.Add(vec3i);
            }
        }


        return doorTiles;
    }

    [Button]
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

    private void CopyTilemap(Tilemap from, Tilemap to, Vector3Int tileOffset)
    {
        var tilesInBounds = from.cellBounds.allPositionsWithin;
        List<Vector3Int> doorTiles = new List<Vector3Int>();
        foreach (var vec3i in tilesInBounds)
        {
            TileBase tile = from.GetTile<TileBase>(vec3i);
            if (tile)
            {
                Vector3Int newTilePos = vec3i + tileOffset;
                to.SetTile(newTilePos, tile);
            }
        }
    }
}
