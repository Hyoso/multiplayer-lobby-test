using System;
using System.Reflection;
using System.Runtime.Serialization;
using TMPro;
using Unity.Services.Friends.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Samples.Friends.UGUI
{
    [System.Serializable]
    public class FriendEntryData
    {
        public string joinCode;
        public string Activity;
        public PresenceAvailabilityOptions availability;
    }

    public class FriendEntryViewUGUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_NameText = null;
        [SerializeField] TextMeshProUGUI m_ActivityText = null;
        [SerializeField] Image m_PresenceColorImage = null;

        public Button joinButton = null;
        public Button leaveButton = null;
        public Button removeFriendButton = null;
        public Button blockFriendButton = null;

        private PresenceAvailabilityOptions m_presence;
        private FriendEntryData m_entryData;

        private void Awake()
        {
            joinButton.onClick.AddListener(() => joinButton.gameObject.SetActive(false));
            leaveButton.onClick.AddListener(() => {

                GameNetworkManager.Instance.DisconnectFromHost();
                UpdateJoinButton();
                leaveButton.gameObject.SetActive(false);
            });

            GameplayEvents.onJoinHostSuccess += GameplayEvents_onJoinHost;
        }

        private void OnDestroy()
        {
            GameplayEvents.onJoinHostSuccess -= GameplayEvents_onJoinHost;
        }

        private void GameplayEvents_onJoinHost(string joinCode)
        {
            if (joinCode == m_entryData.joinCode)
            {
                leaveButton.gameObject.SetActive(true);
            }
        }

        public void Init(string playerName, PresenceAvailabilityOptions presenceAvailabilityOptions, string json)
        {
            m_entryData = JsonUtility.FromJson<FriendEntryData>(json);

            m_NameText.text = playerName;
            var index = (int)presenceAvailabilityOptions - 1;
            var presenceColor = ColorUtils.GetPresenceColor(index);
            m_PresenceColorImage.color = presenceColor;
            m_presence = presenceAvailabilityOptions;
            if (m_entryData.availability == PresenceAvailabilityOptions.OFFLINE)
            {
                m_ActivityText.text = "Last online: " + m_entryData.Activity;
            }
            else
            {
                m_ActivityText.text = GetEnumMemberValue(m_entryData.availability);
            }

            UpdateJoinButton();
        }

        private void UpdateJoinButton()
        {
            bool enableJoinButton = !string.IsNullOrEmpty(m_entryData.joinCode);
            joinButton.gameObject.SetActive(enableJoinButton);
        }

        private string GetEnumMemberValue(Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            MemberInfo memberInfo = enumType.GetMember(enumValue.ToString())[0];

            EnumMemberAttribute enumMemberAttribute = memberInfo.GetCustomAttribute<EnumMemberAttribute>();
            if (enumMemberAttribute != null)
            {
                return enumMemberAttribute.Value;
            }
            return enumValue.ToString(); // Default to the enum's string representation
        }
    }
}