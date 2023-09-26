using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class StateMachineBehaviour
{
    public Action onStateEnterCallback;
    public Action onStateUpdateallback;
    public Action onStateExitCallback;

    public abstract void SetupState();

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
