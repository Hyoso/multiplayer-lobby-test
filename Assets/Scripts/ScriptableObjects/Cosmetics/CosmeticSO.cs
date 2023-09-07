using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CosmeticSO : ScriptableObject
{
    public enum CosmeticType
    {
        HAT,
        BODY,
        FEET,
        TRAIL,
        FEATHER,
    }

    public string cosmeticName;
    public CosmeticType type;
    public Sprite equippedSprite;
    public Sprite icon;
}
