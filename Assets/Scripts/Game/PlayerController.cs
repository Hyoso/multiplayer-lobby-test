using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private GameObject m_spawnedPlayerObject;
    [SerializeField] private GameObject m_playerPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);

            LoadLastState();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            GameNetworkManager.Instance.SetLastPlayerState(new GameNetworkManager.LastPlayerState
            {
                position = m_spawnedPlayerObject.transform.position,
                rotation = m_spawnedPlayerObject.transform.eulerAngles
            });
        }
    }

    // for server respawn after starting host
    private void LoadLastState()
    {
        if (IsServer)
        {
            var lastState = GameNetworkManager.Instance.lastPlayerState;
            m_spawnedPlayerObject.transform.position = lastState.position;
            m_spawnedPlayerObject.transform.eulerAngles = lastState.rotation;
        }
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
}
