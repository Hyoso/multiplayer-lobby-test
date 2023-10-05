using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileNamePlate : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI m_nameText;

    public void SetName(string profileName)
    {
        m_nameText.text = profileName;
    }

    public void Button_PrfoileSelected()
    {
        GameplayEvents.SendNewProfileSelectedEvent(m_nameText.text);
    }
}
