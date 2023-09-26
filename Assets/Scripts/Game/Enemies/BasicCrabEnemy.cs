using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BasicStateMachine))]
public class BasicCrabEnemy : TargetBase
{
    private const string ANIM_WALK_LEFT_FRONT = "WalkLeftFront";
    private const string ANIM_WALK_RIGHT_FRONT = "WalkRightFront";
    private const string ANIM_IDLE_LEFT = "IdleLeft";
    private const string ANIM_IDLE_RIGHT = "IdleRight";

    [SerializeField] private AnimationHelper m_animator;
    [SerializeField] private Rigidbody2D m_rigidbody2D;
    [SerializeField] private BasicStateMachine m_stateMachine;
    


    private void Start()
    {
        SetupStateMachine();
    }

    protected override void PlayDeathSequence()
    {
    }

    private void Update()
    {
        m_stateMachine.UpdateMachine();
    }

    private void SetupStateMachine()
    {
        m_stateMachine = gameObject.GetComponent<BasicStateMachine>();
        m_stateMachine.Setup(m_animator);
        IdleState idleState = new IdleState();
        ChaseState chaseState = new ChaseState();

        m_stateMachine.AddState((int)BasicStateMachine.States.IDLE, idleState);
        m_stateMachine.AddState((int)BasicStateMachine.States.CHASE, chaseState);
    }
}
