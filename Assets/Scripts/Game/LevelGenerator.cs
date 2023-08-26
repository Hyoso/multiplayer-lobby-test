using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour
{
    public Grid m_grid;
    public Tilemap m_dungeonMap;
    public Tilemap m_startRoom;
    public List<Tilemap> m_tiles = new List<Tilemap>();
    public Tile m_doorTile;
    public Tile m_floorTile;

    [Button]
    public void Generate()
    {
        List<Vector3Int> checkDirections = new List<Vector3Int>{
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        m_dungeonMap.ClearAllTiles();
        AddStartRoom();

        /// #### ALGORITHM ### ///
        /// 
        // check start room for available doors
        // add to list
        // choose one at random
        // choose a room at random
        // check chosen room for available doors
        // add to list
        // check which doors can fit with the start room door without overlapping
        // additional: add bounds for the map
        ///
        /// #### ALGORITHM ### ///

        //Tilemap tilemap = m_tiles[0];
        //tilemap.CompressBounds();

        List<Vector3Int> doorTiles = GetDoorTiles(m_dungeonMap, m_doorTile);

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
                Debug.Log("Found valid cell at: " + validCell + " for door at " + randDoor);

                // todo: change m_Startroom to randomly selected one from list later
                Tilemap newRoom = m_startRoom;

                List<Vector3Int> doorsInNewRoom = GetDoorTiles(newRoom, m_doorTile);

                foreach (var doorInNewRoom in doorsInNewRoom)
                {
                    Vector3Int offset = validCell - doorInNewRoom;
                    bool hasOverlap = CheckOverlap(m_dungeonMap, newRoom, offset);
                    if (!hasOverlap)
                    {
                        CopyTilemap(newRoom, m_dungeonMap, offset);
                        break;
                    }
                }
                break;
            }
        }


        //Debug.Log("tilecount: " + count);

        //m_dungeonMap.SetTile(new Vector3Int(1, -1), m_floorTile);


        //string output = "";

        //output = tilemap.WorldToCell(new Vector3(5, 2.1f, 0)).ToString();

        //Debug.Log(output);


        //Debug.Log(tilemap.cellBounds.allPositionsWithin.ToString());
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
                //Debug.Log("Found door tile at - " + vec3i);
                doorTiles.Add(vec3i);
            }
        }


        return doorTiles;
    }

    private bool CheckOverlap(Tilemap tm1, Tilemap tm2, Vector3Int offset)
    {
        var tm1Tiles = tm1.cellBounds.allPositionsWithin;
        foreach (var vec3i in tm1Tiles)
        {
            Vector3Int checkCell = vec3i + offset;
            if (tm2.GetTile(checkCell) != null)
            {
                return true;
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
            Vector3Int newTilePos = vec3i + tileOffset;
            to.SetTile(newTilePos, tile);
        }
    }
}
