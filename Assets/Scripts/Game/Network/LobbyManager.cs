using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    public class Timer
    {
        private float m_timer;
        private float m_maxTime;
        private Func<Task<string>> m_asyncFunction;

        public Timer(float maxTime, Func<Task<string>> asyncFunction)
        {
            m_timer = maxTime;
            m_maxTime = maxTime;
            m_asyncFunction = asyncFunction;
        }

        public async void Update()
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0f)
            {
                m_timer += m_maxTime;
                await m_asyncFunction();
            }
        }
    }

    private const float HOST_HEARTBEAT = 15f;
    private const float POLL_UPDATE_LOBBY_TIME = 1.1f;

    private Timer m_heartbeatTimer = null;
    private Timer m_pollForLobbyUpdatesTimer = null;

    private Lobby m_currentLobby;

    private string m_playerName;

    protected override void Init()
    {
    }

    private async void Start()
    {
        m_playerName = "TEST " + UnityEngine.Random.Range(10, 99);

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        m_heartbeatTimer?.Update();
        m_pollForLobbyUpdatesTimer?.Update();
    }

    [Command]
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "Test Lobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };

            m_currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            StartHeartbeatTimer();
            StartPollForLobbyUpdatesTimer();

            Debug.Log("Created lobby! " + m_currentLobby.Name + " " + m_currentLobby.MaxPlayers);
            PrintPlayers(m_currentLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void ListLobbies()
    {
        try
        {
            // todo: hook up to UI
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };


            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(options);

            Debug.Log("Lobbies found: " + response.Results.Count);
            foreach (var item in response.Results)
            {
                Debug.Log(item.Name + " " + item.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void JoinLobby()
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();

            m_currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(response.Results[0].Id, options);
           
            StartPollForLobbyUpdatesTimer();

            Debug.Log("Joined lobby with name: " + m_currentLobby.Name);
            PrintPlayers(m_currentLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void JoinLobby(string code)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            m_currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, options);
            
            StartPollForLobbyUpdatesTimer();

            Debug.Log("Joined lobby with code: " + code);
            PrintPlayers(m_currentLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };

            m_currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);

            StartPollForLobbyUpdatesTimer();

            Debug.Log("Joined lobby with name: " + m_currentLobby.Name);
            PrintPlayers(m_currentLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(m_currentLobby.Id, AuthenticationService.Instance.PlayerId);
           
            NullTimers();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    // host option only
    [Command]
    private async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(m_currentLobby.Id, m_currentLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        Player player = new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, m_playerName) }
                    }
        };

        return player;
    }

    [Command]
    private void PrintPlayers()
    {
        PrintPlayers(m_currentLobby);
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in " + lobby.Name + " lobby");
        foreach (var item in lobby.Players)
        {
            Debug.Log(item.Id + item.Data["PlayerName"].Value);
        }
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            m_playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(m_currentLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject> {
                {
                    "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, m_playerName)
                } }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void MigrateLobbyHost()
    {
        try
        {
            m_currentLobby = await Lobbies.Instance.UpdateLobbyAsync(m_currentLobby.Id, new UpdateLobbyOptions
            {
                HostId = m_currentLobby.Players[1].Id
            });

            StartHeartbeatTimer();

            PrintPlayers(m_currentLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    [Command]
    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(m_currentLobby.Id);

            NullTimers();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void NullTimers()
    {
        m_heartbeatTimer = null;
        m_pollForLobbyUpdatesTimer = null;
    }

    private void StartHeartbeatTimer()
    {
        m_heartbeatTimer = new Timer(HOST_HEARTBEAT, async () =>
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(m_currentLobby.Id);
               
                return "Sent heartbeat";
            }
            catch (LobbyServiceException e)
            {
                m_currentLobby = null;
                m_heartbeatTimer = null;

                Debug.Log(e);
                return "";
            }
        });
    }

    private void StartPollForLobbyUpdatesTimer()
    {
        m_pollForLobbyUpdatesTimer = new Timer(POLL_UPDATE_LOBBY_TIME, async () =>
        {
            try
            {
                m_currentLobby = await LobbyService.Instance.GetLobbyAsync(m_currentLobby.Id);

                return "Updated lobby data";
            }
            catch (LobbyServiceException e)
            {
                m_currentLobby = null;
                m_pollForLobbyUpdatesTimer = null;

                Debug.Log(e);
                return "";
            }
        });
    }
}
