using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterAnimations : NetworkBehaviour
{
    private const string ANIM_IDLE = "idle";
    private const string ANIM_RUN_LEFT = "run-left";
    private const string ANIM_RUN_RIGHT = "run-right";
    private const string ANIM_RUN_UP = "run-up";
    private const string ANIM_RUN_DOWN = "run-down";


    [SerializeField] private AnimationHelper m_animator;
    
    private Rigidbody2D m_rigidbody;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (m_rigidbody.velocity.magnitude > 0)
        {
            if (m_rigidbody.velocity.y > 0)
            {
                // moving up
                m_animator.PlayAnimation(ANIM_RUN_UP, checkIsPlayingFirst: true);
            }
            else if (m_rigidbody.velocity.y < 0)
            {
                // moving down
                m_animator.PlayAnimation(ANIM_RUN_DOWN, checkIsPlayingFirst: true);
            }
            else if (m_rigidbody.velocity.x  > 0)
            {
                // moving right
                m_animator.PlayAnimation(ANIM_RUN_RIGHT, checkIsPlayingFirst: true);
            }
            else if (m_rigidbody.velocity.x < 0)
            {
                // moving left
                m_animator.PlayAnimation(ANIM_RUN_LEFT, checkIsPlayingFirst: true);
            }
        }
        else
        {
            // idle
            m_animator.PlayAnimation(ANIM_IDLE, checkIsPlayingFirst: true);
        }
    }
}
