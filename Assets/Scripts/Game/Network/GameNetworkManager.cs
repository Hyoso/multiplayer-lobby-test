using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
    [SerializeField] private RelayController m_relayController;
    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_playerPrefab;

    protected override void Init() { }

    private void Start()
    {
        //NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        //response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }

    private void OnClientConnectedCallback(ulong client)
    {
    }

    [Command]
    public void StartOffline()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", 7777);

        NetworkManager.Singleton.StartHost();
    }

    [Command]
    public async void StartHost()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }

        try
        {
            await m_relayController.SignIn();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        try
        {
            await m_relayController.CreateRelay();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    [Command]
    public async void JoinHost(string joinCode)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }

        try
        {
            await m_relayController.SignIn();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        m_relayController.JoinRelay(joinCode);
    }
}
