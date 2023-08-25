using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
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
        m_dungeonMap.ClearAllTiles();
        //AddStartRoom();

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

        Tilemap tilemap = m_tiles[0];
        tilemap.CompressBounds();

        var tilesInBounds = tilemap.cellBounds.allPositionsWithin;
        int count = 0;
        foreach (var vec3i in tilesInBounds)
        {
            Tile tile = tilemap.GetTile<Tile>(vec3i);
            if (tile != null)
            {
                count++;
            }
            if (tile == m_doorTile)
            {
                Debug.Log("Found door tile at - " + vec3i);
            }
        }

        Debug.Log("tilecount: " + count);

        m_dungeonMap.SetTile(new Vector3Int(-3, -3), m_floorTile);


        //string output = "";

        //output = tilemap.WorldToCell(new Vector3(5, 2.1f, 0)).ToString();

        //Debug.Log(output);


        //Debug.Log(tilemap.cellBounds.allPositionsWithin.ToString());
    }

    private void AddStartRoom()
    {
        //m_floorMap.SetTiles(m_startRoom.)
    }
}
