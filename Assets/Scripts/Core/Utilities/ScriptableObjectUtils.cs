using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public static class ScriptableObjectUtils
{
    public static List<T> LoadScriptableObjectsFromFolder<T>(string folderPath) where T : ScriptableObject
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
#endif
