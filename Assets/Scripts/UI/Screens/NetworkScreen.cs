using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkScreen : UIScreen
{
    [SerializeField] private GameObject m_hostBtn;
    [SerializeField] private GameObject m_stopHostBtn;
    [SerializeField] private Button m_interactWithNPCBtn;

    private void Awake()
    {
        GameplayEvents.onOnlineHostStarted += GameplayEvents_onOnlineHostStarted;
        GameplayEvents.onOnlineHostStopped += GameplayEvents_onOnlineHostStopped;
        GameplayEvents.onHostDisconnected += GameplayEvents_onHostDisconnected;
        GameplayEvents.onJoinHostSuccess += GameplayEvents_onJoinHost;

        GameplayEvents.PlayerInNPCRangeEvent += GameplayEvents_PlayerInNPCRangeEvent;
    }

    private void OnDestroy()
    {
        GameplayEvents.onOnlineHostStarted -= GameplayEvents_onOnlineHostStarted;
        GameplayEvents.onOnlineHostStopped -= GameplayEvents_onOnlineHostStopped;
        GameplayEvents.onHostDisconnected -= GameplayEvents_onHostDisconnected;
        GameplayEvents.onJoinHostSuccess -= GameplayEvents_onJoinHost;

        GameplayEvents.PlayerInNPCRangeEvent -= GameplayEvents_PlayerInNPCRangeEvent;
    }

    private void GameplayEvents_PlayerInNPCRangeEvent(bool inRange)
    {
        m_interactWithNPCBtn.interactable = inRange;
    }

    private void GameplayEvents_onHostDisconnected()
    {
        m_hostBtn.SetActive(true);
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

    public void OnInteractNPCButtonDown()
    {
        GameplayEvents.SendInteractWithNPCEvent();
    }

    public void OnUseActiveSkillButtonDown()
    {

    }
}
