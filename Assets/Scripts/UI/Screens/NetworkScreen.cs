using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkScreen : UIScreen
{
    private const string NEXT_WAVE_TEXT = "Next Wave in:";
    private const string WAVE_ENDS_IN_TEXT = "Wave Ends in:";

    [SerializeField] private GameObject m_hostBtn;
    [SerializeField] private GameObject m_stopHostBtn;
    [SerializeField] private Button m_interactWithNPCBtn;
    [SerializeField] private TMPro.TextMeshProUGUI m_timerText;
    [SerializeField] private TMPro.TextMeshProUGUI m_waveInfoText;

    private void Awake()
    {
        GameplayEvents.onOnlineHostStarted += GameplayEvents_onOnlineHostStarted;
        GameplayEvents.onOnlineHostStopped += GameplayEvents_onOnlineHostStopped;
        GameplayEvents.onHostDisconnected += GameplayEvents_onHostDisconnected;
        GameplayEvents.onJoinHostSuccess += GameplayEvents_onJoinHost;

        GameplayEvents.WaveFailedEvent += GameplayEvents_WaveFailedEvent;
        GameplayEvents.WaveCompletedEvent += GameplayEvents_WaveCompletedEvent;
        GameplayEvents.StartWaveEvent += GameplayEvents_StartWaveEvent;
        GameplayEvents.WaveTimerUpdatedEvent += GameplayEvents_WaveTimerUpdatedEvent;
        GameplayEvents.PlayerInNPCRangeEvent += GameplayEvents_PlayerInNPCRangeEvent;
    }

    private void Start()
    {
        WavesManager.WaveState state = (WavesManager.WaveState)WavesManager.Instance.netState.Value;
        if (state == WavesManager.WaveState.COOLDOWN)
        {
            m_waveInfoText.text = NEXT_WAVE_TEXT;
        }
        else if (state == WavesManager.WaveState.SPAWNING)
        {
            m_waveInfoText.text = WAVE_ENDS_IN_TEXT;
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.onOnlineHostStarted -= GameplayEvents_onOnlineHostStarted;
        GameplayEvents.onOnlineHostStopped -= GameplayEvents_onOnlineHostStopped;
        GameplayEvents.onHostDisconnected -= GameplayEvents_onHostDisconnected;
        GameplayEvents.onJoinHostSuccess -= GameplayEvents_onJoinHost;

        GameplayEvents.WaveFailedEvent -= GameplayEvents_WaveFailedEvent;
        GameplayEvents.WaveCompletedEvent -= GameplayEvents_WaveCompletedEvent;
        GameplayEvents.StartWaveEvent -= GameplayEvents_StartWaveEvent;
        GameplayEvents.WaveTimerUpdatedEvent -= GameplayEvents_WaveTimerUpdatedEvent;
        GameplayEvents.PlayerInNPCRangeEvent -= GameplayEvents_PlayerInNPCRangeEvent;
    }

    private void GameplayEvents_StartWaveEvent()
    {
        m_waveInfoText.text = WAVE_ENDS_IN_TEXT;
    }

    private void GameplayEvents_WaveFailedEvent()
    {
        m_waveInfoText.text = NEXT_WAVE_TEXT;
    }

    private void GameplayEvents_WaveCompletedEvent()
    {
        m_waveInfoText.text = NEXT_WAVE_TEXT;
    }

    private void GameplayEvents_WaveTimerUpdatedEvent(int intValue)
    {
        m_timerText.text = intValue.ToString();
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
