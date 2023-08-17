using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupError : Popup
{
    public const string POPUP_PATH = "Prefabs/Popups/Popup-Error";

    [SerializeField] private TMPro.TextMeshProUGUI m_errorTextTMP;
    [SerializeField] private TMPro.TextMeshProUGUI m_closeBtnTextTMP;
    [SerializeField] private Button m_closeBtn;

    private string m_errorMessage;
    private string m_closeBtnText;
    private Action m_onCloseCallback;

    public void Init(string errorMsg, string closeBtnText, Action onClose)
    {
        m_errorMessage = errorMsg;
        m_closeBtnText = closeBtnText;
        m_onCloseCallback = onClose;

        m_errorTextTMP.text = m_errorMessage;
        m_closeBtnTextTMP.text = m_closeBtnText;
        m_closeBtn.onClick.AddListener(() =>
        {
            m_onCloseCallback.Invoke();
            CloseWindow();
        });
    }
}
