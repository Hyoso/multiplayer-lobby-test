using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BasicStateMachine))]
public class BasicCrabEnemy : TargetBase
{

    [SerializeField] private AnimationHelper m_animator;
    [SerializeField] private Rigidbody2D m_rigidbody2D;
    [SerializeField] private BasicStateMachine m_stateMachine;
    
    public override void Start()
    {
        SetupStateMachine();

        base.Start();
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

        m_stateMachine.SetState((int)BasicStateMachine.States.IDLE);
    }
}
