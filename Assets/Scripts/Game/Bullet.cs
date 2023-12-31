using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public bool used { get { return m_used; } }
    public float damage { get { return m_damage; } }

    [SerializeField] private float m_damage = 2f;
    [SerializeField] private float m_speed = 10f;
    [SerializeField] private NetworkSpriteRenderer m_feather;
    [SerializeField] private NetworkTransform m_networkTransform;

    private bool m_used = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();

            StartCoroutine(Coroutines.Delay(3f, () =>
            {
                GetComponent<NetworkObject>().Despawn(true);
            }));

            m_networkTransform.SlerpPosition = false;
            m_feather.ChangeRenderEnabled(false);

            // re-enable interpolate after a frame so position and rotation can be set properly
            StartCoroutine(Coroutines.Delay(0.05f, () =>
            {
                m_networkTransform.SlerpPosition = true;
                m_feather.ChangeRenderEnabled(true);
            }));
        }
    }

    public void HitEnemy(Transform enemy)
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        netObj.Despawn(true);
        m_used = true;
    }

    private void Update()
    {
        if (IsServer)
            transform.position += transform.right * Time.deltaTime * m_speed;
    }

    private void OnValidate()
    {
        if (m_networkTransform == null)
        {
            m_networkTransform = GetComponent<NetworkTransform>();
        }
    }
}
