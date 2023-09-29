using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class WavesManager : NetworkSingleton<WavesManager>
{
    public enum WaveState
    {
        COOLDOWN = 0,
        SPAWNING,
    }

    public NetworkVariable<int> netState = new NetworkVariable<int>();
    public NetworkVariable<int> netTimer = new NetworkVariable<int>();

    public WaveSO temporaryWave;

    private WaveState m_state = WaveState.COOLDOWN;
    private float m_waveTimer;
    private float m_cooldownTimer;
    private int m_waveTimerInt;

    protected override void Init()
    {
        GameplayEvents.StartWaveEvent += GameplayEvents_StartWaveEvent;
        GameplayEvents.WaveCompletedEvent += GameplayEvents_WaveCompletedEvent;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        netState.OnValueChanged += OnNetStateChanged;
        netTimer.OnValueChanged += OnNetTimerChanged;

        if (!IsServer) return;

        m_cooldownTimer = GameplayConfig.Instance.waveCooldown;
        UpdateNetTimer(m_cooldownTimer);
        SetState(WaveState.COOLDOWN);
    }

    private void OnNetTimerChanged(int previousValue, int newValue)
    {
        GameplayEvents.SendWaveTimerUpdatedEvent(newValue);
    }

    private void OnNetStateChanged(int previousValue, int newValue)
    {
        Debug.Log("wave state changed: " + (WaveState)newValue);
    }

    private void GameplayEvents_WaveCompletedEvent()
    {
        m_cooldownTimer = GameplayConfig.Instance.waveCooldown;
        UpdateNetTimer(m_cooldownTimer);
        SetState(WaveState.COOLDOWN);
    }

    private void GameplayEvents_StartWaveEvent()
    {
        m_waveTimer = temporaryWave.duration;
        UpdateNetTimer(m_waveTimer);
        SetState(WaveState.SPAWNING);
        SpawnWave();
    }

    private void Update()
    {
        if (!IsServer) return;

        if (m_state == WaveState.SPAWNING)
        {
            m_waveTimer -= Time.deltaTime;
            UpdateNetTimer(m_waveTimer);

            if (m_waveTimer <= 0f)
            {
                WaveFailedClientRpc();
            }
        }
        else if (m_state == WaveState.COOLDOWN)
        {
            m_cooldownTimer -= Time.deltaTime;
            UpdateNetTimer(m_cooldownTimer);

            if (m_cooldownTimer <= 0f)
            {
                WaveStartedClientRpc();
            }
        }
    }

    public void WaveCompleted()
    {
        if (IsServer)
        {
            WaveCompletedClientRpc();
        }
    }

    [ClientRpc]
    private void WaveStartedClientRpc()
    {
        GameplayEvents.SendStartWaveEvent();
    }

    [ClientRpc]
    private void WaveFailedClientRpc()
    {
        GameplayEvents.SendWaveFailedEvent();
    }

    [ClientRpc]
    private void WaveCompletedClientRpc()
    {
        GameplayEvents.SendWaveCompletedEvent();
    }

    private void UpdateNetTimer(float timer)
    {
        m_waveTimerInt = (int)timer;
        if (m_waveTimerInt != netTimer.Value)
        {
            netTimer.Value = m_waveTimerInt;
        }
    }

    private void SetState(WaveState state)
    {
        m_state = state;
        netState.Value = (int)m_state;
    }

    private void SpawnWave()
    {
        MonsterSpawnManager.Instance.SpawnMonsters(temporaryWave);
    }
}
