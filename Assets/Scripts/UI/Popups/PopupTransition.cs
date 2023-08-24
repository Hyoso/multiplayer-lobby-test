using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTransition : MonoBehaviour
{
    public const string POPUP_PATH = "Prefabs/Popups/Popup-Transition";

    [SerializeField] private TransitionSettings m_transition;

    public void Init(Action onCutPointReached)
    {
        TransitionManager.Instance().onTransitionCutPointReached += () => onCutPointReached?.Invoke();
        TransitionManager.Instance().Transition(m_transition, 0f);
    }
}
