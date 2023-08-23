using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkScreen : UIScreen
{
    [SerializeField] private GameObject m_hostBtn;
    [SerializeField] private GameObject m_stopHostBtn;

    private void Awake()
    {
        GameplayEvents.onOnlineHostStarted += GameplayEvents_onOnlineHostStarted;
        GameplayEvents.onOnlineHostStopped += GameplayEvents_onOnlineHostStopped;
        GameplayEvents.onJoinHostSuccess += GameplayEvents_onJoinHost;
    }

    private void OnDestroy()
    {
        GameplayEvents.onOnlineHostStarted -= GameplayEvents_onOnlineHostStarted;
        GameplayEvents.onOnlineHostStopped -= GameplayEvents_onOnlineHostStopped;
        GameplayEvents.onJoinHostSuccess -= GameplayEvents_onJoinHost;
    }

    private void GameplayEvents_onOnlineHostStopped()
    {
        m_hostBtn.SetActive(true);
    }

    private void GameplayEvents_onOnlineHostStarted(string joinCode)
    {
        m_stopHostBtn.SetActive(true);
    }

    private void GameplayEvents_onJoinHost(string joinCode)
    {
        m_stopHostBtn.SetActive(false);
        m_hostBtn.SetActive(false);
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        GameNetworkManager.Instance.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StopHost()
    {
        GameNetworkManager.Instance.StopHost();
    }
}
