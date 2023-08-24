using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTransition : Popup
{
    public const string POPUP_PATH = "Prefabs/Popups/Popup-Transition";

    [SerializeField] private TransitionSettings m_transition;

    public void Init(Action onCutPointReached, Action onEnd)
    {
        TransitionManager.Instance().onTransitionCutPointReached = () => onCutPointReached?.Invoke();
        TransitionManager.Instance().onTransitionEnd = () => onEnd?.Invoke();
        TransitionManager.Instance().Transition(m_transition, 0f);
    }
}
