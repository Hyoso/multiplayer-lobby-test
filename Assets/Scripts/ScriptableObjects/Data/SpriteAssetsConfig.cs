using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using QFSW.QC.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu]
public class SpriteAssetsConfig : GenericConfig<SpriteAssetsConfig>
{
    [SerializeField]
    private StringAndSpriteDictionary spritesDictionary;

    [Header("Add Element")]
    [SerializeField]
    private string key;
    [SerializeField]
    private Sprite value;

    [Button]
    public void AddElement()
    {
        if (key == null || value == null)
        {
            Debug.LogError("Key and value must be set");
            return;
        }

        spritesDictionary.Add(key, value);
    }

    public Sprite GetSprite(string spriteName)
    {
        if (spritesDictionary.ContainsKey(spriteName))
        {
            return spritesDictionary[spriteName];
        }

        Debug.LogError("could not find sprite with name : " + spriteName);
        return null;
    }
}
