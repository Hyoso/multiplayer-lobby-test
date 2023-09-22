using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera { get { return m_camera; } private set { } }

    [SerializeField] private CinemachineVirtualCamera m_camera;
    [SerializeField] private GameObject m_playerCharacter;

    public void AssignPlayer(GameObject go)
    {
        m_playerCharacter = go;
        m_camera.Follow = m_playerCharacter.transform;
    }
}
