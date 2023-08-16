using NaughtyAttributes;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
    public struct LastPlayerState
    {
        public Vector3 position;
        public Vector3 rotation;

        // anything else
    }

    [SerializeField] private RelayController m_relayController;
    [SerializeField] private ClientConnectionHandler m_connectionHandler;
    [ReadOnly, SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_tmpCharacterDisplayPrefab;

    public bool creatingRelay { get { return m_creatingRelay; } set { } }
    private bool m_creatingRelay;

    public LastPlayerState lastPlayerState { get { return m_lastPlayerState; } set { } }
    private LastPlayerState m_lastPlayerState;

    protected override void Init() { }

    private void Start()
    {
    }

    private void OnDestroy()
    {
    }

    public void SetLastPlayerState(LastPlayerState state)
    {
        m_lastPlayerState = state;
    }

    [Command]
    public void StartOffline()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", 7777);

        NetworkManager.Singleton.StartHost();
    }

    [Command]
    public void StopHost()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }

        StartCoroutine(WaitAndReloadOfflineHost());
    }

    private IEnumerator WaitAndReloadOfflineHost()
    {
        yield return new WaitWhile(() => NetworkManager.Singleton.ShutdownInProgress);

        StartOffline();
    }

    [Command]
    public async void StartHost()
    {
        m_creatingRelay = true;

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }

        while (NetworkManager.Singleton.ShutdownInProgress)
        {
            await Task.Delay(5);
        }

        GameObject temporaryCharacterDisplay = Instantiate(m_tmpCharacterDisplayPrefab);
        temporaryCharacterDisplay.transform.position = m_lastPlayerState.position;
        temporaryCharacterDisplay.transform.eulerAngles = m_lastPlayerState.rotation;

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

        Destroy(temporaryCharacterDisplay);

        m_creatingRelay = false;
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
