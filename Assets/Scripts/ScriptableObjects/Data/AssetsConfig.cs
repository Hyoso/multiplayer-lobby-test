using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

[CreateAssetMenu]
public class AssetsConfig : GenericConfig<AssetsConfig>
{
	public List<Object> levels = new List<Object>();
	[ReadOnly] public List<string> levelNames = new List<string>();

    private void OnValidate()
    {
		levelNames = new List<string>();
		foreach (var item in levels)
		{
			levelNames.Add(item.name);
		}
    }
}
