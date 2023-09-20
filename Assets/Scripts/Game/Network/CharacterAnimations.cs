using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterAnimations : NetworkBehaviour
{
    private enum MoveDirections
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    private const string ANIM_IDLE_LEFT_BACK = "IdleLeftBack";
    private const string ANIM_IDLE_LEFT_FRONT = "IdleLeftFront";
    private const string ANIM_IDLE_RIGHT_BACK = "IdleRightBack";
    private const string ANIM_IDLE_RIGHT_FRONT = "IdleRightFront";
    private const string ANIM_RUN_LEFT_BACK = "RunLeftBack";
    private const string ANIM_RUN_LEFT_FRONT = "RunLeftFront";
    private const string ANIM_RUN_RIGHT_BACK = "RunRightBack";
    private const string ANIM_RUN_RIGHT_FRONT = "RunRightFront";
    private const string DEFAULT_ANIM = ANIM_IDLE_RIGHT_FRONT;


    [SerializeField] private AnimationHelper m_animator;
    
    private Rigidbody2D m_rigidbody;
    private MoveDirections m_verticalDir = MoveDirections.DOWN;
    private MoveDirections m_horizontalDir = MoveDirections.RIGHT;
    private MoveDirections m_lastVerticalDir = MoveDirections.DOWN;
    private MoveDirections m_lastHorizontalDir = MoveDirections.RIGHT;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator.PlayAnimation(DEFAULT_ANIM);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (m_rigidbody.velocity.magnitude > 0)
            UpdateMoveDirection();

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (m_rigidbody.velocity.magnitude > 0)
        {
            if (m_verticalDir == MoveDirections.DOWN && m_horizontalDir == MoveDirections.RIGHT)
            {
                m_animator.PlayAnimation(ANIM_RUN_RIGHT_FRONT, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.DOWN && m_horizontalDir == MoveDirections.LEFT)
            {
                m_animator.PlayAnimation(ANIM_RUN_LEFT_FRONT, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.UP && m_horizontalDir == MoveDirections.RIGHT)
            {
                m_animator.PlayAnimation(ANIM_RUN_RIGHT_BACK, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.UP && m_horizontalDir == MoveDirections.LEFT)
            {
                m_animator.PlayAnimation(ANIM_RUN_LEFT_BACK, checkIsPlayingFirst: true);
            }
        }
        else
        {
            // idle
            if (m_verticalDir == MoveDirections.DOWN && m_horizontalDir == MoveDirections.RIGHT)
            {
                m_animator.PlayAnimation(ANIM_IDLE_RIGHT_FRONT, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.DOWN && m_horizontalDir == MoveDirections.LEFT)
            {
                m_animator.PlayAnimation(ANIM_IDLE_LEFT_FRONT, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.UP && m_horizontalDir == MoveDirections.RIGHT)
            {
                m_animator.PlayAnimation(ANIM_IDLE_RIGHT_BACK, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.UP && m_horizontalDir == MoveDirections.LEFT)
            {
                m_animator.PlayAnimation(ANIM_IDLE_LEFT_BACK, checkIsPlayingFirst: true);
            }
        }
    }

    private void UpdateMoveDirection()
    {
        m_verticalDir = m_lastVerticalDir;
        m_horizontalDir = m_lastHorizontalDir;

        if (m_rigidbody.velocity.y < 0)
        {
            m_verticalDir = MoveDirections.DOWN;
        }
        else if (m_rigidbody.velocity.y > 0)
        {
            m_verticalDir = MoveDirections.UP;
        }

        if (m_rigidbody.velocity.x < 0)
        {
            m_horizontalDir = MoveDirections.LEFT;
        }
        else if (m_rigidbody.velocity.x > 0)
        {
            m_horizontalDir = MoveDirections.RIGHT;
        }

        m_lastVerticalDir = m_verticalDir;
        m_lastHorizontalDir = m_horizontalDir;
    }
}
