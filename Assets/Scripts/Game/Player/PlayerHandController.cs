using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    public Transform featherTransform { get { return m_featherTransform; } private set { } }

    [SerializeField] private Transform m_handTransform;
    [SerializeField] private Transform m_featherTransform;

    private Camera m_camera;

    private void Start()
    {
        m_camera = GameManager.Instance.playerCam.GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = m_handTransform.position.z - m_camera.transform.position.z;
        Vector3 worldMousePos = m_camera.ScreenToWorldPoint(mousePos);
        Vector3 dirToMouse = m_handTransform.position - worldMousePos;
        dirToMouse = -dirToMouse.normalized;

        m_handTransform.right = dirToMouse;
    }
}
