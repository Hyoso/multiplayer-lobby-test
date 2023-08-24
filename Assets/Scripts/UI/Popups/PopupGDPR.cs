using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupGDPR : Popup
{
    // privayc poklicy https://pastebin.com/D8ffb5Q7
    // ToS https://pastebin.com/i0R2vf6b

    public void AcceptGDPR()
    {
        SaveSystem.Instance.SetBool(BucketCore.GDPR_ACCEPTED, "", true);
        SaveSystem.Instance.Save();
       
        CloseWindow();
    }

    public void OpenToS()
    {
        Application.OpenURL(GDPRConfig.Instance.termsOfServiceURL);
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL(GDPRConfig.Instance.privacyPolicyURL);
    }
}
