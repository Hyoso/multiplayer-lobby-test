using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // only server will know this
    [SerializeField] private GameObject m_spawnedPlayerObject;
    [SerializeField] private GameObject m_playerPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            GameManager.Instance.RegisterLocalPlayer(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.RegisterLocalPlayer(null);

        base.OnNetworkDespawn();
    }


    [ServerRpc(RequireOwnership = true)]
    public void SpawnPlayerServerRpc(ulong clientId)
    {
        GameObject player = Instantiate(m_playerPrefab);
        NetworkObject playerNetworkObj = player.GetComponent<NetworkObject>();
        playerNetworkObj.SpawnWithOwnership(clientId);

        m_spawnedPlayerObject = player;
        m_spawnedPlayerObject.name = "Client " + clientId;
    }

    [ServerRpc]
    public void GenerateLevelServerRpc()
    {
        if (IsServer)
        {
            GameplayEvents.SendGenerateNewLevelEvent();
        }
    }
}
