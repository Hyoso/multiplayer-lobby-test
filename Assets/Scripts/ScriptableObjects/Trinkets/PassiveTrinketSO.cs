using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PassiveTrinketSO : TrinketSO
{
    public override TrinketType GetTrinketType()
    {
        return TrinketType.PASSIVE;
    }

    protected override void UseActive()
    {
    }
}
