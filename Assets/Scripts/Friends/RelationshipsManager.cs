using Mono.CSharp;
using QFSW.QC.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Friends;
using Unity.Services.Friends.Exceptions;
using Unity.Services.Friends.Models;
using Unity.Services.Samples.Friends.UGUI;
using UnityEngine;
using VContainer;

namespace Unity.Services.Samples.Friends
{
    public class RelationshipsManager : MonoBehaviour
    {
        public const string DEFAULT_ACTIVITY = "ONLINE";
        
        //This gameObject reference is only needed to get the IRelationshipUIController component from it.
        [Tooltip("Reference a GameObject that has a component extending from IRelationshipsUIController."), SerializeField]
        GameObject m_RelationshipsViewGameObject;

        IRelationshipsView m_RelationshipsView;

        List<FriendsEntryData> m_FriendsEntryDatas = new List<FriendsEntryData>();
        List<PlayerProfile> m_RequestsEntryDatas = new List<PlayerProfile>();
        List<PlayerProfile> m_BlockEntryDatas = new List<PlayerProfile>();

        ILocalPlayerView m_LocalPlayerView;
        IAddFriendView m_AddFriendView;
        IFriendsListView m_FriendsListView;
        IRequestListView m_RequestListView;
        IBlockedListView m_BlockListView;

        PlayerProfile m_LoggedPlayerProfile;
        static FriendEntryData m_localEntryData;


        AuthenticationServiceFacade m_AuthServiceFacade;

        private void Awake()
        {
            m_AuthServiceFacade = new AuthenticationServiceFacade();
            m_localEntryData = new FriendEntryData();
            GameplayEvents.onOnlineHostStarted += GameplayEvents_onOnlineHostStarted;
            GameplayEvents.onOnlineHostStopped += GameplayEvents_onOnlineHostStopped;
            GameplayEvents.ProfileChangedEvent += GameplayEvents_ProfileChangedEvent;
        }

        async void Start()
        {
            //If this is added to a larger project, the service init order should be controlled from one place, and replace this.
            await UnityServiceAuthenticator.SignIn();
            await Init();
        }

        async Task Init()
        {
            await FriendsService.Instance.InitializeAsync();
            UIInit();
            await LogInAsync();
            SubscribeToFriendsEventCallbacks();
            RefreshAll();
        }

        void UIInit()
        {
            if (m_RelationshipsViewGameObject == null)
            {
                Debug.LogError($"Missing GameObject in {name}", gameObject);
                return;
            }

            m_RelationshipsView = m_RelationshipsViewGameObject.GetComponent<IRelationshipsView>();
            if (m_RelationshipsView == null)
            {
                Debug.LogError($"No Component extending IRelationshipsView {m_RelationshipsViewGameObject.name}",
                    m_RelationshipsViewGameObject);
                return;
            }

            m_RelationshipsView.Init();
            m_LocalPlayerView = m_RelationshipsView.LocalPlayerView;
            m_AddFriendView = m_RelationshipsView.AddFriendView;

            //Bind Lists
            m_FriendsListView = m_RelationshipsView.FriendsListView;
            m_FriendsListView.BindList(m_FriendsEntryDatas);
            m_RequestListView = m_RelationshipsView.RequestListView;
            m_RequestListView.BindList(m_RequestsEntryDatas);
            m_BlockListView = m_RelationshipsView.BlockListView;
            m_BlockListView.BindList(m_BlockEntryDatas);

            //Bind Friends SDK Callbacks
            m_AddFriendView.onFriendRequestSent += AddFriendAsync;
            m_FriendsListView.onRemove += RemoveFriendAsync;
            m_FriendsListView.onBlock += BlockFriendAsync;
            m_FriendsListView.onJoinFriend += JoinFriendAsync;
            m_RequestListView.onAccept += AcceptRequestAsync;
            m_RequestListView.onDecline += DeclineRequestAsync;
            m_RequestListView.onBlock += BlockFriendAsync;
            m_BlockListView.onUnblock += UnblockFriendAsync;
            m_LocalPlayerView.onPresenceChanged += SetPresenceAsync;
        }

        private void OnDestroy()
        {
            GameplayEvents.onOnlineHostStarted -= GameplayEvents_onOnlineHostStarted;
            GameplayEvents.onOnlineHostStopped -= GameplayEvents_onOnlineHostStopped;
        }

        private async void GameplayEvents_onOnlineHostStarted(string joinCode)
        {
            m_localEntryData.joinCode = joinCode;
            string friendEntryDataJson = JsonUtility.ToJson(m_localEntryData);

            await SetPresence(PresenceAvailabilityOptions.ONLINE, friendEntryDataJson);
        }

        private async void GameplayEvents_onOnlineHostStopped()
        {
            m_localEntryData.joinCode = "";
            string friendEntryDataJson = JsonUtility.ToJson(m_localEntryData);

            await SetPresence(PresenceAvailabilityOptions.ONLINE, friendEntryDataJson);
        }

        async Task LogInAsync()
        {
            try
            {
                await OnAuthSignIn();
                await UpdateFriendsScreen();
            }
            catch (System.Exception)
            {
                OnSignInFailed();
            }


            //await AuthenticationService.Instance.UpdatePlayerNameAsync("TESTER");

        }

        private async Task OnAuthSignIn()
        {
            //m_LobbyButton.interactable = true;
            //m_UGSSetupTooltipDetector.enabled = false;
            //m_SignInSpinner.SetActive(false);

            var unityAuthenticationInitOptions =
                m_AuthServiceFacade.GenerateAuthenticationOptions(ProfilesManager.Instance.Profile);

            await m_AuthServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);

            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");

            var playerID = AuthenticationService.Instance.PlayerId;
            var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            m_LoggedPlayerProfile = new PlayerProfile(playerName, playerID);
            Debug.Log($"Logged in as {m_LoggedPlayerProfile}");
        }

        private async Task UpdateFriendsScreen()
        {
            m_localEntryData.Activity = DEFAULT_ACTIVITY;
            m_localEntryData.availability = PresenceAvailabilityOptions.ONLINE;
            string friendEntryDataJson = JsonUtility.ToJson(m_localEntryData);

            await SetPresence(PresenceAvailabilityOptions.ONLINE, friendEntryDataJson);
            m_LocalPlayerView.Refresh(
                m_LoggedPlayerProfile.Name,
                DEFAULT_ACTIVITY,
                PresenceAvailabilityOptions.ONLINE);
            RefreshAll();
        }

        private void OnSignInFailed()
        {
            Debug.LogError("Sign in failed");
        }

        private async void GameplayEvents_ProfileChangedEvent()
        {
            await m_AuthServiceFacade.SwitchProfileAndReSignInAsync(ProfilesManager.Instance.Profile);

            var playerID = AuthenticationService.Instance.PlayerId;
            var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            m_LoggedPlayerProfile = new PlayerProfile(playerName, playerID);
            Debug.Log($"Logged in as {m_LoggedPlayerProfile}");

            await FriendsService.Instance.InitializeAsync();

            await LogInAsync();
            SubscribeToFriendsEventCallbacks();

            await UpdateFriendsScreen();
        }

        void RefreshAll()
        {
            RefreshFriends();
            RefreshRequests();
            RefreshBlocks();
        }

        async void JoinFriendAsync(string joinCode)
        {
            await JoinFriend(joinCode);
        }

        async void BlockFriendAsync(string id)
        {
            await BlockFriend(id);
            RefreshAll();
        }

        async void UnblockFriendAsync(string id)
        {
            await UnblockFriend(id);
            RefreshBlocks();
            RefreshFriends();
        }

        async void RemoveFriendAsync(string id)
        {
            await RemoveFriend(id);
            RefreshFriends();
        }

        async void AcceptRequestAsync(string name)
        {
            await AcceptRequest(name);
            RefreshRequests();
            RefreshFriends();
        }

        async void DeclineRequestAsync(string id)
        {
            await DeclineRequest(id);
            RefreshRequests();
        }

        async void SetPresenceAsync((PresenceAvailabilityOptions presence, string activity) status)
        {
            m_localEntryData.Activity = status.activity;
            m_localEntryData.availability = status.presence;
            string friendEntryDataJson = JsonUtility.ToJson(m_localEntryData);

            await SetPresence(status.presence, friendEntryDataJson);
            m_LocalPlayerView.Refresh(m_LoggedPlayerProfile.Name, status.activity, status.presence);
        }

        async void AddFriendAsync(string name)
        {
            var success = await SendFriendRequest(name);
            if (success)
                m_AddFriendView.FriendRequestSuccess();
            else
                m_AddFriendView.FriendRequestFailed();
        }

        void RefreshFriends()
        {
            m_FriendsEntryDatas.Clear();

            var friends = GetFriends();

            foreach (var friend in friends)
            {
                string friendEntryDataJson;
                if (friend.Presence.Availability == PresenceAvailabilityOptions.OFFLINE ||
                    friend.Presence.Availability == PresenceAvailabilityOptions.INVISIBLE)
                {
                    string lastActivity = friend.Presence.LastSeen.ToShortDateString() + " " +
                                   friend.Presence.LastSeen.ToLongTimeString();

                    FriendEntryData data = new FriendEntryData();
                    data.Activity = lastActivity;
                    data.availability = friend.Presence.Availability;
                    friendEntryDataJson = JsonUtility.ToJson(data);
                }
                else
                {
                    string activity = friend.Presence.GetActivity<Activity>() == null
                        ? ""
                        : friend.Presence.GetActivity<Activity>().Status;

                    friendEntryDataJson = activity;
                }

                var info = new FriendsEntryData
                {
                    Name = friend.Profile.Name,
                    Id = friend.Id,
                    Availability = friend.Presence.Availability,
                    Activity = friendEntryDataJson,
                };
                m_FriendsEntryDatas.Add(info);
            }

            m_RelationshipsView.RelationshipBarView.Refresh();
        }

        void RefreshRequests()
        {
            m_RequestsEntryDatas.Clear();
            var requests = GetRequests();

            foreach (var request in requests)
                m_RequestsEntryDatas.Add(new PlayerProfile(request.Profile.Name, request.Id));

            m_RelationshipsView.RelationshipBarView.Refresh();
        }

        void RefreshBlocks()
        {
            m_BlockEntryDatas.Clear();

            foreach (var block in FriendsService.Instance.Blocks)
                m_BlockEntryDatas.Add(new PlayerProfile(block.Member.Profile.Name, block.Member.Id));

            m_RelationshipsView.RelationshipBarView.Refresh();
        }

        async Task JoinFriend(string joinCode)
        {
            GameNetworkManager.Instance.JoinHost(joinCode);
            await Task.Delay(0);
        }

        async Task<bool> SendFriendRequest(string playerName)
        {
            try
            {
                //We add the friend by name in this sample but you can also add a friend by ID using AddFriendAsync
                var relationship = await FriendsService.Instance.AddFriendByNameAsync(playerName);
                Debug.Log($"Friend request sent to {playerName}.");
                return relationship.Type == RelationshipType.FRIEND_REQUEST;
            }
            catch (RelationshipsServiceException e)
            {
                Debug.Log($"Failed to Request {playerName} - {e}.");
                return false;
            }
        }

        async Task RemoveFriend(string playerId)
        {
            try
            {
                await FriendsService.Instance.DeleteFriendAsync(playerId);
                Debug.Log($"{playerId} was removed from the friends list.");
            }
            catch (RelationshipsServiceException e)
            {
                Debug.Log($"Failed to remove {playerId}.");
                Debug.LogError(e);
            }
        }

        async Task BlockFriend(string playerId)
        {
            try
            {
                await FriendsService.Instance.AddBlockAsync(playerId);
                Debug.Log($"{playerId} was blocked.");
            }
            catch (RelationshipsServiceException e)
            {
                Debug.Log($"Failed to block {playerId}.");
                Debug.LogError(e);
            }
        }

        async Task UnblockFriend(string playerId)
        {
            try
            {
                await FriendsService.Instance.DeleteBlockAsync(playerId);
                Debug.Log($"{playerId} was unblocked.");
            }
            catch (RelationshipsServiceException e)
            {
                Debug.Log($"Failed to unblock {playerId}.");
                Debug.LogError(e);
            }
        }

        async Task AcceptRequest(string playerName)
        {
            try
            {
                await SendFriendRequest(playerName);
                Debug.Log($"Friend request from {playerName} was accepted.");
            }
            catch (RelationshipsServiceException e)
            {
                Debug.Log($"Failed to accept request from {playerName}.");
                Debug.LogError(e);
            }
        }

        async Task DeclineRequest(string playerId)
        {
            try
            {
                await FriendsService.Instance.DeleteIncomingFriendRequestAsync(playerId);
                Debug.Log($"Friend request from {playerId} was declined.");
            }
            catch (RelationshipsServiceException e)
            {
                Debug.Log($"Failed to decline request from {playerId}.");
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Get an amount of friends (including presence data).
        /// </summary>
        /// <returns>List of friends.</returns>
        List<Member> GetFriends()
        {
            return GetNonBlockedMembers(FriendsService.Instance.Friends);
        }

        /// <summary>
        /// Get an amount of Requests. The friends SDK maintains relationships unless explicitly deleted, even those
        /// towards blocked players. We don't want to show blocked players' requests, so we filter them out.
        /// </summary>
        /// <returns>List of players.</returns>
        List<Member> GetRequests()
        {
            return GetNonBlockedMembers(FriendsService.Instance.IncomingFriendRequests);
        }

        async Task SetPresence(PresenceAvailabilityOptions presenceAvailabilityOptions,
            string activityStatus = "")
        {
            var activity = new Activity { Status = activityStatus };

            try
            {
                await FriendsService.Instance.SetPresenceAsync(presenceAvailabilityOptions, activity);
                Debug.Log($"Availability changed to {presenceAvailabilityOptions}.");
            }
            catch (RelationshipsServiceException e)
            {
                Debug.Log($"Failed to set the presence to {presenceAvailabilityOptions}");
                Debug.LogError(e);
            }
        }

        void SubscribeToFriendsEventCallbacks()
        {
            try
            {
                FriendsService.Instance.RelationshipAdded += e =>
                {
                    RefreshRequests();
                    RefreshFriends();
                    Debug.Log($"create {e.Relationship} EventReceived");
                };
                FriendsService.Instance.MessageReceived += e =>
                {
                    RefreshRequests();
                    Debug.Log("MessageReceived EventReceived");
                };
                FriendsService.Instance.PresenceUpdated += e =>
                {
                    RefreshFriends();
                    Debug.Log("PresenceUpdated EventReceived");
                };
                FriendsService.Instance.RelationshipDeleted += e =>
                {
                    RefreshFriends();
                    Debug.Log($"Delete {e.Relationship} EventReceived");
                };
            }
            catch (RelationshipsServiceException e)
            {
                Debug.Log(
                    "An error occurred while performing the action. Code: " + e.Reason + ", Message: " + e.Message);
            }
        }

        /// <summary>
        /// Returns a list of members that are not blocked by the active user.
        /// </summary>
        /// <param name="relationships">The list of relationships to filter.</param>
        /// <returns>Filtered list of members.</returns>
        private List<Member> GetNonBlockedMembers(IReadOnlyList<Relationship> relationships)
        {
            var blocks = FriendsService.Instance.Blocks;
            return relationships
                   .Where(relationship =>
                       !blocks.Any(blockedRelationship => blockedRelationship.Member.Id == relationship.Member.Id))
                   .Select(relationship => relationship.Member)
                   .ToList();
        }
    }
}
