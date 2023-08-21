using TMPro;
using Unity.Services.Friends.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Samples.Friends.UGUI
{
    public class FriendEntryViewUGUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_NameText = null;
        [SerializeField] TextMeshProUGUI m_ActivityText = null;
        [SerializeField] Image m_PresenceColorImage = null;

        public Button joinButton = null;
        public Button leaveButton = null;
        public Button removeFriendButton = null;
        public Button blockFriendButton = null;

        private string m_joinCode;
        private string m_activity;
        private PresenceAvailabilityOptions m_presence;

        private void Awake()
        {
            joinButton.onClick.AddListener(() => joinButton.gameObject.SetActive(false));
            leaveButton.onClick.AddListener(() => {

                GameNetworkManager.Instance.DisconnectFromHost();
                UpdateJoinButton();
                leaveButton.gameObject.SetActive(false);
            });

            GameplayEvents.onJoinHost += GameplayEvents_onJoinHost;
        }

        private void OnDestroy()
        {
            GameplayEvents.onJoinHost -= GameplayEvents_onJoinHost;
        }

        private void GameplayEvents_onJoinHost(string joinCode)
        {
            if (joinCode == m_joinCode)
            {
                leaveButton.gameObject.SetActive(true);
            }
        }

        public void Init(string playerName, PresenceAvailabilityOptions presenceAvailabilityOptions, string activity)
        {
            m_NameText.text = playerName;
            var index = (int)presenceAvailabilityOptions - 1;
            var presenceColor = ColorUtils.GetPresenceColor(index);
            m_PresenceColorImage.color = presenceColor;
            m_ActivityText.text = activity;
            m_presence = presenceAvailabilityOptions;
            m_activity = activity;

            UpdateJoinButton();
        }

        private void UpdateJoinButton()
        {
            if (m_presence == PresenceAvailabilityOptions.ONLINE)
            {
                m_joinCode = m_activity;
                joinButton.gameObject.SetActive(m_activity != RelationshipsManager.DEFAULT_ACTIVITY);
            }
            else
            {
                joinButton.gameObject.SetActive(false);
            }
        }
    }
}