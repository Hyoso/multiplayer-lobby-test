using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LagTextDisplay : MonoBehaviour
{
    private TMPro.TextMeshProUGUI m_text;

    private void Start()
    {
        m_text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsConnectedClient)
            return;


        var serverTime = NetworkManager.Singleton.ServerTime;
        var localTime = NetworkManager.Singleton.LocalTime;

        var timeDiff = localTime - serverTime;

        m_text.text = (timeDiff.TimeAsFloat * 1000f).ToString("F0") + "ms";
    }
}
