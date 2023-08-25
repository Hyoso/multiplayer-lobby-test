using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsScreen : UIScreen
{
    [SerializeField] private CanvasGroup m_relationshipsPanel;

    public void ToggleView()
    {
        if (m_relationshipsPanel.interactable)
        {
            m_relationshipsPanel.interactable = false;
            m_relationshipsPanel.alpha = 0f;
            m_relationshipsPanel.blocksRaycasts = false;
        }
        else
        {
            m_relationshipsPanel.interactable = true;
            m_relationshipsPanel.alpha = 1f;
            m_relationshipsPanel.blocksRaycasts = true;
        }
    }
}
