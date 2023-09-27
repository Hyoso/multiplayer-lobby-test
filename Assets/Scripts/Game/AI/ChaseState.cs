using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : StateMachineBehaviour
{
    public float speed = 2f;

    private Transform m_transform;
    private Transform m_target;
    private Rigidbody2D m_rigidbody;

    public void SetTransform(Transform transform)
    {
        m_transform = transform;
        m_rigidbody = m_transform.GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Transform target)
    {
        if (m_target != target)
        {
            m_target = target;
        }
    }

    public override void OnStateEnter()
    {
        m_target = null;
        m_rigidbody.velocity = Vector3.zero;
    }

    public override void OnStateExit()
    {
        m_rigidbody.velocity = Vector3.zero;
    }

    public override void OnStateUpdate()
    {
        if (m_target)
        {
            Vector3 dirToTarget = m_target.position-  m_transform.position;
            dirToTarget = dirToTarget.normalized;

            m_rigidbody.velocity = dirToTarget * Time.deltaTime * speed;
        }
    }

    public override void SetupState()
    {
    }
}
