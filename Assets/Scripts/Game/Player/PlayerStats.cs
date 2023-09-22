using UnityEngine;

public struct PlayerStats
{
    public enum StatType
    {
        NONE,
        HEALTH,
        DAMAGE,
        RANGE,
        ATTACK_SPEED,
        MOVE_SPEED,
        LUCK
    }

    public float health;
    public float damage;
    public float range;
    public float attackSpeed;
    public float moveSpeed;
    public float luck;

    public float GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.HEALTH: return health;
            case StatType.DAMAGE: return damage;
            case StatType.RANGE: return range;
            case StatType.ATTACK_SPEED: return attackSpeed;
            case StatType.MOVE_SPEED: return moveSpeed;
            case StatType.LUCK: return luck;
            case StatType.NONE:
                Debug.Log("Must use a valid skill type");
                break;
        }

        return -1;
    }
}
