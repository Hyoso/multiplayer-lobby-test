using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ActiveTrinketSO : TrinketSO
{
    public float cooldown { get { return m_cooldown; } }
    [SerializeField] private float m_cooldown;

    private float m_cooldownTimer;

    public void TryUseActive()
    {
        if (CanUseActive() && GetTrinketType() == TrinketType.ACTIVE)
        {
            UseActive();
        }
    }

    public override TrinketType GetTrinketType()
    {
        return TrinketType.ACTIVE;
    }

    public override void Update()
    {
        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        if (m_cooldownTimer > 0)
        {
            m_cooldownTimer -= Time.deltaTime;
        }
    }

    protected override void UseActive()
    {
        m_cooldownTimer = m_cooldown;
        Debug.Log("Used active");
    }

    protected bool CanUseActive()
    {
        bool canUseActive = m_cooldownTimer <= 0;
        return canUseActive;
    }
}
