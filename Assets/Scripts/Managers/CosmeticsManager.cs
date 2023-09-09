using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CosmeticsManager : Singleton<CosmeticsManager>
{
    [SerializeField] private List<CosmeticSO> m_cosmetics = new List<CosmeticSO>();

    private List<string> cosmeticFolders = new List<string>()
    {
        "Body",
        "Feathers",
        "Feet",
        "Hats",
        "Trails"
    };

    protected override void Init()
    {
    }

    public Sprite GetSprite(string spriteName)
    {
        CosmeticSO cosmetic = m_cosmetics.Find((x) => x.equippedSprite.name == spriteName);
        if (cosmetic != null)
        {
            return cosmetic.equippedSprite;
        }

        return null;
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        string folderPath = "Assets/Data/Cosmetics/";

        List<CosmeticSO> scriptableObjects = new List<CosmeticSO>();

        foreach (var folder in cosmeticFolders)
        {
            string path = folderPath + folder;
            scriptableObjects.AddRange(ScriptableObjectUtils.LoadScriptableObjectsFromFolder<CosmeticSO>(path));
        }

        foreach (CosmeticSO obj in scriptableObjects)
        {
            if (!m_cosmetics.Contains(obj))
            {
                m_cosmetics.Add(obj);
            }
        }
#endif
    }
}
