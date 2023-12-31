using NaughtyAttributes;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(BasicStateMachine))]
public class BasicCrabEnemy : TargetBase
{
    [SerializeField] private AnimationHelper m_animator;
    [SerializeField] private Rigidbody2D m_rigidbody2D;
    [SerializeField] private BasicStateMachine m_stateMachine;

    private void Awake()
    {
        GameplayEvents.WaveFailedEvent += GameplayEvents_WaveFailedEvent;
    }

    public override void Start()
    {
        SetupStateMachine();

        base.Start();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameplayEvents.WaveFailedEvent -= GameplayEvents_WaveFailedEvent;
    }

    protected override void PlayDeathSequence()
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        netObj.Despawn(true);
    }

    private void Update()
    {
        if (IsServer)
        {
            m_stateMachine.UpdateMachine();
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            m_stateMachine.FixedUpdateMachine();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsServer)
        {
            if (m_health.Value > 0 && collision.CompareTag(Tags.PlayerBullet))
            {
                Bullet bullet = collision.GetComponent<Bullet>();
                if (!bullet.used)
                {
                    TakeDamage(bullet.damage);
                    bullet.HitEnemy(transform);
                }
            }
        }
    }

    private void GameplayEvents_WaveFailedEvent()
    {
        if (IsServer)
        {
            PlayDeathSequence();
        }
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
