using System.Linq;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Generic Configuration class
/// Use this base class to create game configuration scriptable objects
/// 
/// This script will automatically create an instance and save it to the specified path in editor mode
/// 
/// Usage Example:
/// <code>
/// public class GameplayConfig : GenericConfig<T>
/// {
///		public float someFloat;
/// }
/// </code>
/// 
/// To get an instance of the GameplayConfig use:
/// <code>
/// GameplayConfig.Instance;
/// </code>
/// 
/// </summary>

public abstract class GenericConfig<T> : ScriptableObject where T  : GenericConfig<T>
{
	private const string CONFIG_LOCATION = "Configs";

	private static T s_instance;

	public static T Instance
	{
		get
		{
			if (s_instance == null)
			{
				Load();

				// failed to find a config, creating one - only in editor mode
				if (s_instance == null)
				{
					CreateConfig();
				}
			}
			return s_instance;
		}
		set
		{
			s_instance = value;
		}
	}

	private static void Load()
	{
		s_instance = Resources.LoadAll<T>(CONFIG_LOCATION).FirstOrDefault();
	}

	private static void CreateConfig()
	{
#if UNITY_EDITOR
		string typeName = typeof(T).ToString();
		GenericConfig<T> asset = ScriptableObject.CreateInstance<T>();
		string savePath = string.Format("Assets/Data/Resources/{0}/", CONFIG_LOCATION);
		string assetName = string.Format("{0}.asset", typeName);
		ValidateDirectory(savePath);

		AssetDatabase.CreateAsset(asset, savePath + assetName);
		AssetDatabase.SaveAssets();

		s_instance = (T)asset;

		Debug.LogError("Failed to find " + typeName + " - created a new one");
#endif
	}

	private static void ValidateDirectory(string directory)
	{
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
	}
}
