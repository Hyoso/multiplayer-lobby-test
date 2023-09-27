using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : StateMachineBehaviour
{
    public float speed = 2f;

    private Transform m_transform;
    private Transform m_target;

    public void SetTransform(Transform transform)
    {
        m_transform = transform;
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
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
        if (m_target)
        {
            Vector3 dirToTarget = m_target.position-  m_transform.position;
            dirToTarget = dirToTarget.normalized;

            m_transform.position += dirToTarget * Time.deltaTime * speed;
        }
    }

    public override void SetupState()
    {
    }
}
