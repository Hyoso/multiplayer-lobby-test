using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PopupExpeditionMenu : Popup
{
    public const string POPUP_PATH = "Prefabs/Popups/Popup-ExpeditionMenu";

    public override void CloseWindow()
    {
        LoadWorld();

        base.CloseWindow();
    }

    private void LoadWorld()
    {
        PlayerController controller = GameManager.Instance.localPlayer;

        controller.GenerateLevelServerRpc();
    }
}
