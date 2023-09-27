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

    private const string ANIM_WALK_LEFT_FRONT = "WalkLeftFront";
    private const string ANIM_WALK_RIGHT_FRONT = "WalkRightFront";
    private const string ANIM_IDLE_LEFT = "IdleLeft";
    private const string ANIM_IDLE_RIGHT = "IdleRight";

    [SerializeField] private float m_range = 3f;
    [SerializeField] private float m_speed = 420f;

    private Rigidbody2D m_rigidbody;
    private Transform m_currentTargetTransform;
    private float m_checkPlayerInRangeTimer;
    private bool m_facingRight = true;

    private ChaseState m_chaseState;

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

        CheckPlayerInRange(); 

        // check if player in range, if not then idle
        if (m_currentTargetTransform == null)
        {
            SetState((int)States.IDLE);
        }
        else
        {
            // player in range, chase player
            SetState((int)States.CHASE);
            m_chaseState.SetTarget(m_currentTargetTransform);
        }
    }

    public override void SetState(int stateId)
    {
        if (stateId == (int)States.IDLE)
        {
            if (m_facingRight)
            {
                m_animator.PlayAnimation(ANIM_IDLE_RIGHT, checkIsPlayingFirst: true);
            }
            else
            {
                m_animator.PlayAnimation(ANIM_IDLE_LEFT, checkIsPlayingFirst: true);
            }
        }
        else if (stateId == (int)States.CHASE)
        {
            if (m_facingRight)
            {
                m_animator.PlayAnimation(ANIM_WALK_RIGHT_FRONT, checkIsPlayingFirst: true);
            }
            else
            {
                m_animator.PlayAnimation(ANIM_WALK_LEFT_FRONT, checkIsPlayingFirst: true);
            }
        }

        base.SetState(stateId);
    }

    public override void AddState(int stateId, StateMachineBehaviour state)
    {
        base.AddState(stateId, state);

        if (state is ChaseState)
        {
            m_chaseState = (ChaseState)state;
            m_chaseState.SetTransform(this.transform);
            m_chaseState.speed = m_speed;
        }
    }

    private void CheckPlayerInRange()
    {
        m_checkPlayerInRangeTimer -= Time.deltaTime;
        if (m_checkPlayerInRangeTimer <= 0f)
        {
            m_currentTargetTransform = PlayerObjectsManager.Instance.GetClosestPlayerToPoint(transform.position, m_range);
            m_checkPlayerInRangeTimer = GameplayConfig.Instance.checklayerInRangeCooldown;
        }
    }
}
