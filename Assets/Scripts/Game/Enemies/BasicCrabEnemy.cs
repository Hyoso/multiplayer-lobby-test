using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCrabEnemy : TargetBase
{
    private const string ANIM_WALK_LEFT_FRONT = "WalkLeftFront";
    private const string ANIM_WALK_RIGHT_FRONT = "WalkRightFront";
    private const string ANIM_IDLE_LEFT = "IdleLeft";
    private const string ANIM_IDLE_RIGHT = "IdleRight";

    [SerializeField] private AnimationHelper m_animator;

    private Transform m_currentTargetTransform;

    protected override void PlayDeathSequence()
    {
    }

    private void Update()
    {
        
    }
}
