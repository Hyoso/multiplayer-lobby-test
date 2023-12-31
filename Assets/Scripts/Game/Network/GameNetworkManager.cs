using EasyTransition;
using NaughtyAttributes;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Relay;
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

    public string joinCode { get { return m_joinCode; } private set { } }
    private string m_joinCode;

    public bool creatingRelay { get { return m_creatingRelay; } private set { } }
    private bool m_creatingRelay;

    public LastPlayerState lastPlayerState { get { return m_lastPlayerState; } private set { } }
    private LastPlayerState m_lastPlayerState;

    protected override void Init() { }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void OnDestroy()
    {
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            DoDisconnectedFromHostTransition();
        }
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
        StopHostSequence();
        GameplayEvents.SendonOnlineHostStopped();
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

        StopHostSequence();

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
    public void DisconnectFromHost()
    {
        NetworkManager.Singleton.Shutdown();

        ProjectSceneManager.Instance.UnloadScene(GameNetworkSceneManager.Instance.SceneName);

        // restart offline gameplay sequence
        StartCoroutine(WaitAndReloadOfflineHost());
    }

    [Command]
    public void JoinHost(string joinCode)
    {
        // show transition
        PopupTransition transition = PopupsManager.Instance.CreatePopup(PopupTransition.POPUP_PATH).GetComponent<PopupTransition>();
        transition.Init(async () =>
        {
            TransitionManager.Instance().pauseTransitionAtCutPoint = true;

            m_joinCode = "";
            // unload clients game world
            m_lastPlayerState = new LastPlayerState();
            GameNetworkSceneManager.Instance.UnloadScene();
            GameplayEvents.SendonJoinHostAttempt();
            StopHostSequence();

            try
            {
                await m_relayController.SignIn();
            }
            catch (AuthenticationException e)
            {
                transition.CloseWindow();
                Debug.LogException(e);
            }

            try
            {
                await m_relayController.JoinRelay(joinCode);
                m_joinCode = joinCode;
                GameplayEvents.SendonJoinHostSuccess(m_joinCode);
            }
            catch (RelayServiceException e)
            {
                transition.CloseWindow();
                Debug.Log(e);
            }

            // this is now in GameNetworkSceneManager.OnNetworkSpawn
            // TransitionManager.Instance().pauseTransitionAtCutPoint = false;
        }, 
        () =>
        {
            transition.CloseWindow();
        });
    }

    private void DoDisconnectedFromHostTransition()
    {
        // show transition
        PopupTransition transition = PopupsManager.Instance.CreatePopup(PopupTransition.POPUP_PATH).GetComponent<PopupTransition>();
        transition.Init(async () =>
        {
            TransitionManager.Instance().pauseTransitionAtCutPoint = true;
            // server shutting down
            ProjectSceneManager.Instance.UnloadScene(GameNetworkSceneManager.Instance.SceneName);

            await AsyncTaskUtils.WaitUntil(() => !NetworkManager.Singleton.ShutdownInProgress);
            
            StartOffline();

            GameplayEvents.SendonHostDisconnected();

            Debug.Log("Host Disconnected");

            TransitionManager.Instance().pauseTransitionAtCutPoint = false;
        },
        () =>
        {
            transition.CloseWindow();
        });
    }

    private void StopHostSequence()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
