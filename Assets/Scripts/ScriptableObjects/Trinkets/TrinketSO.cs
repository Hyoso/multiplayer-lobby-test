using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public abstract class TrinketSO : ScriptableObject
{
    public enum TrinketType
    {
        ACTIVE,
        PASSIVE,
    }

    public string displayName;
    public Sprite icon;
    public float cooldown;
    public PlayerStats boostToPlayerStats = new PlayerStats();
    public List<TrinketSO> requirements = new List<TrinketSO>();
    public List<CosmeticSO> cosmetics = new List<CosmeticSO>();

    public abstract TrinketType GetTrinketType();
    protected abstract void UseActive();

    public void TryUseActive()
    {
        if (CanUseActive() && GetTrinketType() == TrinketType.ACTIVE)
        {
            UseActive();
        }
    }

    public void UpdateCooldown()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }

    public bool CanUnlock()
    {
        return true;
    }

    public bool Unlock()
    {
        return true;
    }

    public bool IsUnlocked()
    {
        return true;
    }

    protected bool CanUseActive()
    {
        bool canUseActive = cooldown <= 0;
        return canUseActive;
    }
}
