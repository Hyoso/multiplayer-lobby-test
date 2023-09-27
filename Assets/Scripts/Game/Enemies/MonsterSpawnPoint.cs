using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterSpawnPoint : NetworkBehaviour
{
    [SerializeField] private List<Transform> spawnableMonsters = new List<Transform>();

    private void Start()
    {
        if (IsServer)
        {
            MonsterSpawnManager.Instance.RegisterSpawnPoint(this);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        if (IsServer)
        {
            MonsterSpawnManager.Instance.DeRegisterSpawnPoint(this);
        }
    }

    public void SpawnMonster()
    {
        Transform monster = spawnableMonsters.GetRandom();

        Transform spawnedMonster = Instantiate(monster);
        spawnedMonster.transform.position = transform.position;

        NetworkObject netObj = spawnedMonster.GetComponent<NetworkObject>();
        netObj.Spawn(true);
    }
}
