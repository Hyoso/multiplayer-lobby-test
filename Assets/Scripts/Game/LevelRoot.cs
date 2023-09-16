using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

public class LevelRoot : NetworkBehaviour
{
    [SerializeField] private GameObject m_levelGenerator;

    [SerializeField, ReadOnly] private GameObject m_spawnedLevelGenerator;

	private void Awake()
	{
		GameManager.Instance.RegisterLevelRoot(this);
		GameplayEvents.SendLevelLoadedEvent();
	}

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer && IsClient)
        {
            SpawnLevelGeneratorServerRpc();
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("Despawned level root");
        //NetworkObject networkObj = m_spawnedLevelGenerator.GetComponent<NetworkObject>();
        //networkObj.Despawn(true);

        base.OnNetworkDespawn();
    }

    [ServerRpc]
    public void SpawnLevelGeneratorServerRpc()
    {
        m_spawnedLevelGenerator = Instantiate(m_levelGenerator);
        NetworkObject networkObj = m_spawnedLevelGenerator.GetComponent<NetworkObject>();
        networkObj.Spawn(true);
        networkObj.TrySetParent(transform);
    }
}
