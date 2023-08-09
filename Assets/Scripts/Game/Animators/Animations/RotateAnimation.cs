using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimation : AnimationBase
{
    [SerializeField] private float m_speed;
    [SerializeField] private float m_targetRotationAmount;

    private float m_rotationCount;
    private Vector3 m_startRotation;

    public override void Init()
    {
        m_startRotation = transform.eulerAngles;
        m_rotationCount = 0f;
    }

    public override bool UpdateAnimation()
    {
        float rotateAmount = m_speed * Time.deltaTime;
        m_rotationCount += rotateAmount;
        

        if (Mathf.Abs(m_rotationCount) > Mathf.Abs(m_targetRotationAmount))
        {
            Vector3 newRotation = m_startRotation;
            newRotation.z += m_targetRotationAmount;

            transform.eulerAngles = newRotation;

            return true;
        }
        else
        {
            transform.Rotate(Vector3.forward, rotateAmount);
        }

        return false;
    }

    public override void ResetSettings()
    {
    }
}
