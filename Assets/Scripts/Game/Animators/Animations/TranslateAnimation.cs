using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateAnimation : AnimationBase
{
    [SerializeField] private Vector3 m_targetPosition;
    [SerializeField] private float m_moveSpeed;

    private Vector3 m_startPosition;

    public override bool UpdateAnimation()
    {
        float moveAmount = m_moveSpeed * Time.deltaTime;

        Vector3 newPos = Vector3.MoveTowards(transform.position, m_targetPosition, moveAmount);
        transform.position = newPos;

        float distToTarget = Vector3.Distance(transform.position, m_targetPosition);
        if (distToTarget <= float.Epsilon)
        {
            transform.position = m_targetPosition;

            return true;
        }

        return false;
    }

    public override void ResetSettings()
    {
    }
}
