using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WaveSO : ScriptableObject
{
    public List<GameObject> waveMonsters = new List<GameObject>();
    public float duration = 15f;
    public int minSpawnCount = 3;
    public int maxSpawnCount = 10;

    public GameObject GetRandomMonster()
    {
        GameObject monster = waveMonsters.GetRandom();
        return monster;
    }

    public int GetRandomSpawnCount()
    {
        int spawnCount = Random.Range(3, 10);
        return spawnCount;
    }
}
