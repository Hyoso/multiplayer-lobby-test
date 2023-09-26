using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicStateMachine : StateMachine
{
    public enum States
    {
        IDLE = 0,
        CHASE,
    }

    private Rigidbody2D m_rigidbody;
    private Transform m_currentTargetTransform;
    private float m_checkPlayerInRangeTimer;
    private bool m_facingRight = true;

    public override void Setup(AnimationHelper animator)
    {
        base.Setup(animator);

        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void UpdateMachine()
    {
        /// states
        /// 1. check player in range
        /// 2. once player is in range - follow unitl out of range or dead


        base.UpdateMachine();

        if (m_currentTargetTransform != null)
        {
            m_checkPlayerInRangeTimer -= Time.deltaTime;
            if (m_checkPlayerInRangeTimer <= 0f)
            {
                m_currentTargetTransform = PlayerObjectsManager.Instance.GetClosestPlayerToPoint(transform.position);
                m_checkPlayerInRangeTimer = GameplayConfig.Instance.checklayerInRangeCooldown;
            }
        }

    }

    public override void SetState(int stateId)
    {
        base.SetState(stateId);

        if (stateId == (int)States.IDLE)
        {

        }
    }
}
