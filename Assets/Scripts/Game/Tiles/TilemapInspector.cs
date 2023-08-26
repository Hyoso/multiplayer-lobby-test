using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapInspector : MonoBehaviour
{
    [SerializeField, ReadOnly] private Tilemap m_tilemap;

    private void OnValidate()
    {
        if (m_tilemap == null)
        {
            m_tilemap = GetComponent<Tilemap>();
        }
    }

    [Button]
    public void ClearAllTiles()
    {
        m_tilemap.ClearAllTiles();
        m_tilemap.CompressBounds();
    }
}
