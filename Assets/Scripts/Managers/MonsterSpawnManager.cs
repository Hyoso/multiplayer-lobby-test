using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnManager : NetworkSingleton<MonsterSpawnManager>
{
    [SerializeField, ReadOnly] private List<MonsterSpawnPoint> m_monsterSpawnPoints = new List<MonsterSpawnPoint>();

    protected override void Init()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnMonsters(1);
        }
    }

    public void SpawnMonsters(int count)
    {
        if (count >= m_monsterSpawnPoints.Count)
        {
            Debug.LogError("Cannot spawn that many, not enough spawn points");
        }

        List<MonsterSpawnPoint> spawnPointsCpy = new List<MonsterSpawnPoint>(m_monsterSpawnPoints);
        for (int i = 0; i < count; i++)
        {
            MonsterSpawnPoint spawnPoint = spawnPointsCpy.GetRandomElementAndRemove();
            spawnPoint.SpawnMonster();
        }
    }

    public void RegisterSpawnPoint(MonsterSpawnPoint spawnPoint)
    {
        if (m_monsterSpawnPoints.Contains(spawnPoint))
        {
            Debug.LogError("spawn point exists");
            return;
        }

        m_monsterSpawnPoints.Add(spawnPoint);
    }

    public void DeRegisterSpawnPoint(MonsterSpawnPoint spawnPoint)
    {
        if (m_monsterSpawnPoints.Contains(spawnPoint))
        {
            m_monsterSpawnPoints.Remove(spawnPoint);
        }
    }
}
