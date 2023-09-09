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

    private void OnValidate()
    {
#if UNITY_EDITOR
        string folderPath = "Assets/Data/Cosmetics/";

        List<CosmeticSO> scriptableObjects = new List<CosmeticSO>();

        foreach (var folder in cosmeticFolders)
        {
            string path = folderPath + folder;
            scriptableObjects.AddRange(LoadScriptableObjectsFromFolder<CosmeticSO>(path));
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


    public Sprite GetSprite(string spriteName)
    {
        CosmeticSO cosmetic = m_cosmetics.Find((x) => x.equippedSprite.name == spriteName);
        if (cosmetic != null)
        {
            return cosmetic.equippedSprite;
        }

        return null;
    }

    private List<T> LoadScriptableObjectsFromFolder<T>(string folderPath) where T : ScriptableObject
    {
        List<T> loadedObjects = new List<T>();

        // Find all assets of the specified type in the folder
        string[] assetPaths = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { folderPath });

        foreach (string assetPath in assetPaths)
        {
            // Load the Scriptable Object from the asset path
            T loadedObject = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetPath));

            if (loadedObject != null)
            {
                loadedObjects.Add(loadedObject);
            }
        }

        return loadedObjects;
    }
}
