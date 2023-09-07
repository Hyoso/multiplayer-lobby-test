using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ActiveTrinketSO : TrinketSO
{
    public override TrinketType GetTrinketType()
    {
        return TrinketType.ACTIVE;
    }

    protected override void UseActive()
    {
        Debug.Log("Used active");
    }
}
