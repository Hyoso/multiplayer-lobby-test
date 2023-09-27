using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class TargetBase : NetworkBehaviour
{
    protected abstract void PlayDeathSequence();

    public Vector3 position { get { return transform.position; } private set { } }
    public bool isAlive { get { return m_health.Value > 0; } }

    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField, ReadOnly] protected NetworkVariable<float> m_health = new NetworkVariable<float>(5f);

    public virtual void Start()
    {
        TargetsManager.Instance.RegisterTarget(this);
    }

    public override void OnDestroy()
    {
        TargetsManager.Instance.UnRegisterTarget(this);

        base.OnDestroy();
    }

    public override void OnNetworkSpawn()
    {
    }

    public void TakeDamage(float damage)
    {
        m_health.Value -= damage;
        if (m_health.Value <= 0) 
        {
            PlayDeathSequence(); 
        }
    }

    public void OnValidate()
    {
        if (m_spriteRenderer == null)
        {
            m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (m_spriteRenderer == null)
            {
                m_spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            }
        }
    }

    public void SetAsTarget()
    {
        m_spriteRenderer.color = Color.red;
    }

    public void ResetTargetView()
    {
        m_spriteRenderer.color = Color.white;
    }

}
