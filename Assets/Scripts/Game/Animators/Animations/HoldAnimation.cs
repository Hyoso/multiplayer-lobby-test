using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldAnimation : AnimationBase
{
    [SerializeField] private float m_holdDuration = 1f;

    private float m_timer;


    public override bool UpdateAnimation()
    {
        m_timer += Time.deltaTime;

        if (m_timer > m_holdDuration)
        {
            return true;
        }

        return false;
    }

    public override void ResetSettings()
    {
        m_timer = 0f;
    }
}
