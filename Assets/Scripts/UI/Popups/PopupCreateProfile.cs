using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PopupCreateProfile : Popup
{
    public const string POPUP_PATH = "Prefabs/Popups/Popup-CreateProfile";

    const int AuthenticationMaxProfileLength = 30;

    [SerializeField] private TMPro.TMP_InputField m_inputField;
    [SerializeField] private Button m_createProfileButton;
    [SerializeField] private ProfileNamePlate m_namePlatePrefab;
    [SerializeField] private Transform m_profilesContent;
    [SerializeField] private CanvasGroup m_canvasGroup;

    private List<ProfileNamePlate> m_profilesList = new List<ProfileNamePlate>();
    
    public void PopulateProfiles()
    {
        // create all profiles
        EnsureNumberOfActiveUISlots(ProfilesManager.Instance.AvailableProfiles.Count);
        for (var i = 0; i < ProfilesManager.Instance.AvailableProfiles.Count; i++)
        {
            var profileName = ProfilesManager.Instance.AvailableProfiles[i];
            m_profilesList[i].SetName(profileName);
        }

        // destroy example
        //Destroy(m_namePlatePrefab.gameObject);
        m_namePlatePrefab.gameObject.SetActive(false);
    }

    /// <summary>
    /// Added to the InputField component's OnValueChanged callback for the join code text.
    /// </summary>
    public void SanitizeProfileNameInputText()
    {
        m_inputField.text = SanitizeProfileName(m_inputField.text);
        m_createProfileButton.interactable = m_inputField.text.Length > 0 && !ProfilesManager.Instance.AvailableProfiles.Contains(m_inputField.text);
    }

    string SanitizeProfileName(string dirtyString)
    {
        var output = Regex.Replace(dirtyString, "[^a-zA-Z0-9]", "");
        return output[..System.Math.Min(output.Length, AuthenticationMaxProfileLength)];
    }

    public void CreateProfile()
    {
        var profile = m_inputField.text;
        if (!ProfilesManager.Instance.AvailableProfiles.Contains(profile))
        {
            ProfilesManager.Instance.CreateProfile(profile);
            ProfilesManager.Instance.Profile = profile;

            AddProfileToList(profile);
        }
        else
        {
            //PopupManager.ShowPopupPanel("Could not create new Profile", "A profile already exists with this same name. Select one of the already existing profiles or create a new one.");
        }
    }

    public void InitializeUI()
    {
        PopulateProfiles();
        //m_EmptyProfileListLabel.enabled = ProfilesManager.Instance.AvailableProfiles.Count == 0;
    }

    private void AddProfileToList(string profileName)
    {
        CreateProfileListItem();
        GameObject profile = m_profilesList.Last().gameObject;
        profile.SetActive(true);

        ProfileNamePlate plate = profile.GetComponent<ProfileNamePlate>();
        plate.SetName(profileName);
    }

    void EnsureNumberOfActiveUISlots(int requiredNumber)
    {
        int delta = requiredNumber - m_profilesList.Count;

        for (int i = 0; i < delta; i++)
        {
            CreateProfileListItem();
        }

        for (int i = 0; i < m_profilesList.Count; i++)
        {
            m_profilesList[i].gameObject.SetActive(i < requiredNumber);
        }
    }

    void CreateProfileListItem()
    {
        var listItem = Instantiate(m_namePlatePrefab.gameObject, m_profilesContent)
            .GetComponent<ProfileNamePlate>();
        m_profilesList.Add(listItem);
        listItem.gameObject.SetActive(true);
        //m_Resolver.Inject(listItem);
    }

    public void Show()
    {
        m_canvasGroup.alpha = 1f;
        m_canvasGroup.blocksRaycasts = true;
        m_inputField.text = "";
        InitializeUI();
    }

    public void Hide()
    {
        m_canvasGroup.alpha = 0f;
        m_canvasGroup.blocksRaycasts = false;
    }
}
