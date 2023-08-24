using EasyTransition;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkSceneManager : NetworkSingleton<GameNetworkSceneManager>
{
    protected override void Init() { }

#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }
#endif

    [SerializeField] private string m_SceneName;

    private Scene m_LoadedScene;

    public bool SceneIsLoaded
    {
        get
        {
            if (m_LoadedScene.IsValid() && m_LoadedScene.isLoaded)
            {
                return true;
            }
            return false;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer && !string.IsNullOrEmpty(m_SceneName) && !m_LoadedScene.isLoaded)
        {
            NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
            CheckStatus(status);
        }

        base.OnNetworkSpawn();

        TransitionManager.Instance().pauseTransitionAtCutPoint = false;
    }

    private void CheckStatus(SceneEventProgressStatus status, bool isLoading = true)
    {
        var sceneEventAction = isLoading ? "load" : "unload";
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to {sceneEventAction} {m_SceneName} with" +
                $" a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }

    /// <summary>
    /// Handles processing notifications when subscribed to OnSceneEvent
    /// </summary>
    /// <param name="sceneEvent">class that has information about the scene event</param>
    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadComplete:
                {
                    // We want to handle this for only the server-side
                    if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        // *** IMPORTANT ***
                        // Keep track of the loaded scene, you need this to unload it
                        m_LoadedScene = sceneEvent.Scene;
                    }
                    Debug.Log($"Loaded the {sceneEvent.SceneName} scene on " +
                        $"{clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
            case SceneEventType.UnloadComplete:
                {
                    Debug.Log($"Unloaded the {sceneEvent.SceneName} scene on " +
                        $"{clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
            case SceneEventType.LoadEventCompleted:
            case SceneEventType.UnloadEventCompleted:
                {
                    var loadUnload = sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ? "Load" : "Unload";
                    Debug.Log($"{loadUnload} event completed for the following client " +
                        $"identifiers:({sceneEvent.ClientsThatCompleted})");
                    if (sceneEvent.ClientsThatTimedOut.Count > 0)
                    {
                        Debug.LogWarning($"{loadUnload} event timed out for the following client " +
                            $"identifiers:({sceneEvent.ClientsThatTimedOut})");
                    }
                    break;
                }
        }
    }

    public void UnloadScene(bool forceUnload = false)
    {
        // Assure only the server calls this when the NetworkObject is
        // spawned and the scene is loaded.
     
        // force if client disconnects
        if (!forceUnload)
        {
            if (!IsServer || !IsSpawned || !m_LoadedScene.IsValid() || !m_LoadedScene.isLoaded)
            {
                return;
            }
        }
        // still need to do additional checks to avoid crash
        else
        {
            if (!IsSpawned || !m_LoadedScene.IsValid())
            {
                return;
            }
        }

        // Unload the scene
        var status = NetworkManager.SceneManager.UnloadScene(m_LoadedScene);
        CheckStatus(status, false);
    }
}
