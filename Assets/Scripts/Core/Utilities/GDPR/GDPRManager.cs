using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDPRManager : Singleton<GDPRManager>
{
    private const string GDPR_POPUP_PATH = "Prefabs/Popups/Popup-GDPR";
    protected override void Init()
    {
    }

    public void CheckShowGDPR()
    {
        bool gdprAccepted = SaveSystem.Instance.GetBool(BucketCore.GDPR_ACCEPTED, "", false);

        if (!gdprAccepted)
        {
            PopupsManager.Instance.CreatePopup(GDPR_POPUP_PATH);
        }
    }
}
