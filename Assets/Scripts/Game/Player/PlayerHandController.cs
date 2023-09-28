using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHandController : NetworkBehaviour
{
    public bool hasTarget { get { return m_hasTarget; } }
    public Transform featherTransform { get { return m_featherTransform; } private set { } }

    [SerializeField] private Transform m_handTransform;
    [SerializeField] private Transform m_featherTransform;

    private Camera m_camera;
    private bool m_hasTarget;

    private void Start()
    {
        m_camera = GameManager.Instance.playerCam.GetComponent<Camera>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            TargetBase closestTarget = TargetsManager.Instance.currentTarget;
            Vector3 targetPosition;
            if (closestTarget == null)
            {
                targetPosition = transform.position + Vector3.right;
            }
            else
            {
                targetPosition = closestTarget.position;
            }

            m_hasTarget = closestTarget != null;

            // target as mouse pos
            //Vector3 mousePos = Input.mousePosition;
            //mousePos.z = m_handTransform.position.z - m_camera.transform.position.z;
            //targetPosition = m_camera.ScreenToWorldPoint(mousePos);

            Vector3 dirToMouse = m_handTransform.position - targetPosition;
            dirToMouse = -dirToMouse.normalized;
            if (transform.position.x > targetPosition.x)
            {
                dirToMouse = -dirToMouse;
            }

            m_handTransform.right = dirToMouse;
        }
    }
}
