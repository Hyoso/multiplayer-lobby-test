using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class AssetsConfig : GenericConfig<AssetsConfig>
{
	[Header("Level system")]
	public List<Object> levels = new List<Object>();
	[ReadOnly] public List<string> levelNames = new List<string>();

	[Header("Network")]
    public NetworkPrefabsList networkObjects;
	[ReadOnly, SerializeField] private List<string> networkObjectsListDisplay = new List<string>(); // inspector only
    [ReadOnly] public Dictionary<string, int> networkObjectDictionary = new Dictionary<string, int>();


    private void OnValidate()
    {
		levelNames = new List<string>();
		foreach (var item in levels)
		{
			levelNames.Add(item.name);
		}

        networkObjectDictionary = new Dictionary<string, int>();
		networkObjectsListDisplay = new List<string>();

        for (int i = 0; i < networkObjects.PrefabList.Count; i++)
		{
			networkObjectDictionary.Add(networkObjects.PrefabList[i].Prefab.name, i);
			networkObjectsListDisplay.Add(networkObjects.PrefabList[i].Prefab.name);
        }
    }


	public GameObject GetNetworkObjectWithName(string name)
	{
		int idx = networkObjectDictionary[name];

        NetworkPrefab prefab = networkObjects.PrefabList[idx];
		GameObject go = prefab.Prefab;

		return go;
    }
}
