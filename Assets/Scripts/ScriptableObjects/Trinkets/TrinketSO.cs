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
    public PlayerStats boostToPlayerStats = new PlayerStats();
    public List<TrinketSO> requirements = new List<TrinketSO>();
    public List<CosmeticSO> cosmetics = new List<CosmeticSO>();

    public abstract TrinketType GetTrinketType();
    public abstract void Update();
    protected abstract void UseActive();


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
}
