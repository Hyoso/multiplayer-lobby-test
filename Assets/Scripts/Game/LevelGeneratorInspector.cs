using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorInspector : MonoBehaviour
{
    [SerializeField, ReadOnly] private LevelGenerator m_generator;

    private void OnValidate()
    {
        if (m_generator == null)
        {
            m_generator = GetComponent<LevelGenerator>();
        }
    }

    [Button]
    public void Generate()
    {
        m_generator.GenerateAsync();
    }
}
