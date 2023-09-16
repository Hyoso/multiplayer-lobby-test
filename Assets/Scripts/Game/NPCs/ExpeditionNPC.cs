using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ExpeditionNPC : NetworkBehaviour
{
    private bool m_playerInRange;

    private void Awake()
    {
        GameplayEvents.InteractWithNPCEvent += GameplayEvents_InteractWithNPCEvent;
    }

    public override void OnDestroy()
    {
        GameplayEvents.InteractWithNPCEvent -= GameplayEvents_InteractWithNPCEvent;
        
        base.OnDestroy();
    }

    private void GameplayEvents_InteractWithNPCEvent()
    {
        if (IsClient)
        {
            GameObject go = PopupsManager.Instance.CreatePopup(PopupExpeditionMenu.POPUP_PATH);
            PopupExpeditionMenu menu = go.GetComponent<PopupExpeditionMenu>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            NetworkObject obj = collision.GetComponent<NetworkObject>();
            if (obj != null && obj.IsOwner)
            {
                m_playerInRange = true;
                GameplayEvents.SendPlayerInNPCRangeEvent(m_playerInRange);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            NetworkObject obj = collision.GetComponent<NetworkObject>();
            if (obj != null && obj.IsOwner)
            {
                m_playerInRange = false;
                GameplayEvents.SendPlayerInNPCRangeEvent(m_playerInRange);
            }
        }
    }
}
