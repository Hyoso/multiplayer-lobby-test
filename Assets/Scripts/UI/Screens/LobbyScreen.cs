using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreen : UIScreen
{
    [SerializeField] private Button m_serverBtn;
    [SerializeField] private Button m_hostBtn;
    [SerializeField] private Button m_clientBtn;

    private void Awake()
    {
        
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
