using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationBase : MonoBehaviour
{
    public abstract bool UpdateAnimation();
    public abstract void ResetSettings();

    public virtual void Init()
    {
    }
}
