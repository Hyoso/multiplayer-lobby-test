using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : Singleton<SaveSystem>
{

	[System.Serializable]
	private class ListSaveHolder<T>
	{
		public List<T> savedList;
	}

	[System.Serializable]
	private class TemplateDictionary<K, V>
	{
		[SerializeField] public List<K> key;
		[SerializeField] public List<V> value;
	}

	private Dictionary<string, BucketBase> m_keyBucketDict;
	private Dictionary<string, BucketBase> m_storageKeyBucketDict;

	protected List<BucketBase> m_buckets;

	protected override void Init()
	{

		m_buckets = new List<BucketBase>();
		m_keyBucketDict = new Dictionary<string, BucketBase>();
		m_storageKeyBucketDict = new Dictionary<string, BucketBase>();

		CreateBuckets();
		CreateBucketDictionary();

		LoadFromDisk();
	}

	protected virtual void CreateBuckets()
	{
		// add any core buckets

		if (CoreConfig.Instance.projectBuckets.Count > 0)
		{
			List<BucketBase> bucketsToAdd = CoreConfig.Instance.projectBuckets;

			foreach (BucketBase bucket in bucketsToAdd)
			{
				AddBucket(bucket);
			}
		}
	}

	private void AddBucket(BucketBase bucket, bool allowReplaceBucket = false)
	{
		if (!m_storageKeyBucketDict.ContainsKey(bucket.GetBucketKey()))
		{
			m_buckets.Add(bucket);
			m_storageKeyBucketDict.Add(bucket.GetBucketKey(), bucket);
		}
		else if (allowReplaceBucket)
		{
			BucketBase bucketToRemove = m_storageKeyBucketDict[bucket.GetBucketKey()];
			m_buckets.Remove(bucketToRemove);

			m_buckets.Add(bucket);
			m_storageKeyBucketDict[bucket.GetBucketKey()] = bucket;
		}
		else
		{
			Debug.LogError("Bucket already exists");
		}
	}

	private void CreateBucketDictionary()
	{
		foreach (var bucket in m_buckets)
		{
			bucket.AddKeys(m_keyBucketDict);
		}
	}

	private BucketBase GetBucketForKey(string paramKey)
	{
		if (m_keyBucketDict.ContainsKey(paramKey))
		{
			return m_keyBucketDict[paramKey];
		}

		Debug.LogError("No key found for: " + paramKey);
		return null;
	}

	public long GetLong(string key, string subKey = "", long defaultValue = 0)
	{
		BucketBase bucket = GetBucketForKey(key);

		if (bucket != null)
		{
			string fullKey = !string.IsNullOrEmpty(subKey) ? key + subKey : key;

			bool bucketHasKey = false;
			object oValue = bucket.GetValue(fullKey, defaultValue, ref bucketHasKey);

			long lValue = defaultValue;
			if (oValue.GetType() == typeof(string))
			{
				string value = (string)oValue;
				long.TryParse(value, out lValue);
			}

			return lValue;
		}

		return defaultValue;
	}

	public bool GetBool(string key, string subKey = "", bool defaultValue = false)
	{
		return GetInt(key, subKey, defaultValue ? 1 : 0) == 1;
	}

	public float GetFloat(string key, string subKey = "", float defaultValue = 0f)
	{
		BucketBase bucket = GetBucketForKey(key);
		
		if (bucket != null)
		{
			string fullKey = !string.IsNullOrEmpty(subKey) ? key + subKey : key;

			bool bucketHasKey = false;
			float value = Convert.ToSingle(bucket.GetValue(fullKey, defaultValue, ref bucketHasKey));

			if (!bucketHasKey && PlayerPrefs.HasKey(key))
			{
				value = PlayerPrefs.GetFloat(key, defaultValue);
			}

			return value;
		}

		return defaultValue;
	}

	public string GetString(string key, string subKey = "", string defaultValue = "")
	{
		BucketBase bucket = GetBucketForKey(key);

		if (bucket != null)
		{
			string fullKey = !string.IsNullOrEmpty(subKey) ? key + subKey : key;

			bool bucketHasKey = false;
			string value = (string)bucket.GetValue(fullKey, defaultValue, ref bucketHasKey);

			if (!bucketHasKey && PlayerPrefs.HasKey(key))
			{
				value = PlayerPrefs.GetString(key, defaultValue);
			}

			return value;
		}

		return defaultValue;
	}

	public int GetInt(string key, string subKey = "", int defaultValue = 0)
	{
		BucketBase bucket = GetBucketForKey(key);

		if (bucket != null)
		{
			string fullKey = !string.IsNullOrEmpty(subKey) ? key + subKey : key;

			bool bucketHasKey = false;
			int value = (int)bucket.GetValue(fullKey, defaultValue, ref bucketHasKey);

			if (!bucketHasKey && PlayerPrefs.HasKey(key))
			{
				value = PlayerPrefs.GetInt(key, defaultValue);
			}

			return value;
		}

		return defaultValue;
	}

	public List<T> GetList<T>(string key, string subKey = "")
	{
		List<T> output = new List<T>();

		try
		{
			string saveData = GetString(key, subKey, "");
			if (!string.IsNullOrEmpty(saveData))
			{
				ListSaveHolder<T> listSaveHolder = JsonUtility.FromJson<ListSaveHolder<T>>(saveData);
				if (listSaveHolder != null)
				{
					output = listSaveHolder.savedList;
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Failed to get list from loading: " + e.ToString());
		}

		return output;
	}

	public T GetObject<T>(string key, string subKey = "")
	{
        try
        {
            string saveData = GetString(key, subKey, "");
            if (!string.IsNullOrEmpty(saveData))
            {
                T output = JsonUtility.FromJson<T>(saveData);
                if (output != null)
                {
					return output;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to get list from loading: " + e.ToString());
        }

        return default(T);
    }

	public ScriptableObject GetScriptableObject<T>(string key, string subKey = "")
	{
        try
        {
            string saveData = GetString(key, subKey, "");
            if (!string.IsNullOrEmpty(saveData))
            {
				var scriptableObject = ScriptableObject.CreateInstance(typeof(T));
				JsonUtility.FromJsonOverwrite(saveData, scriptableObject);
                if (scriptableObject != null)
                {
                    return scriptableObject;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to get list from loading: " + e.ToString());
        }

		return null;
    }

    public Dictionary<K, V> GetDictionary<K, V>(string key, string subKey = "")
	{
		Dictionary<K, V> output = new Dictionary<K, V>();

		try
		{
			string saveData = GetString(key, subKey, "");
			if (!string.IsNullOrEmpty(saveData))
			{
				TemplateDictionary<K, V> templateClass = JsonUtility.FromJson<TemplateDictionary<K, V>>(saveData);
				if (templateClass != null)
				{
					for (int i = 0; i < templateClass.key.Count; i++)
					{
						output.Add(templateClass.key[i], templateClass.value[i]);
					}
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Failed to get dictionary from loading: " + e.ToString());
		}

		return output;
	}

	public void  SetLong(string key, string subKey, long value)
	{
		string sValue = value.ToString();
		SetValue(key, subKey, sValue);
	}

	public void SetBool(string key, string subKey, bool value)
	{
		SetValue(key, subKey, value ? 1 : 0);
	}

	public void SetFloat(string key, string subKey, float value)
	{
		SetValue(key, subKey, value);
	}

	public void SetString(string key, string subKey, string value)
	{
		SetValue(key, subKey, value);
	}

	public void SetInt(string key, string subKey, int value)
	{
		SetValue(key, subKey, value);
	}

	private void SetValue(string key, string subKey, object value)
	{
		BucketBase bucket = GetBucketForKey(key);

		if (bucket != null)
		{
			string fullKey = !string.IsNullOrEmpty(subKey) ? key + subKey : key;
			bucket.SetValue(fullKey, value);
			Save();
        }
    }

	public void SetList<T>(string key, string subKey, List<T> list)
	{
		try
		{
			ListSaveHolder<T> listSaveHolder = new ListSaveHolder<T>();
			listSaveHolder.savedList = list;
			string dataString = JsonUtility.ToJson(listSaveHolder);
			SetString(key, subKey, dataString);
			Save();
        }
        catch (Exception e)
		{
			Debug.LogError("Failed to set list: " + e.ToString());
		}
	}

    public void SetObject<T>(string key, T obj, string subKey = "")
    {
        try
        {
            string dataString = JsonUtility.ToJson(obj, true);
            SetString(key, subKey, dataString);
            Save();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to set object: " + e.ToString());
        }
    }

    public void SetScriptableObject<T>(string key, T obj, string subKey = "")
    {
		SetObject<T>(key, obj, subKey);
    }

    public void SetDictionary<K, V>(string key, string subKey, Dictionary<K, V> dictionary)
	{
		try
		{
			TemplateDictionary<K, V> dictionaryList = new TemplateDictionary<K, V>
			{
				key = new List<K>(dictionary.Keys),
				value = new List<V>(dictionary.Values)
			};

			string dataString = JsonUtility.ToJson(dictionaryList);
			SetString(key, subKey, dataString);
			Save();
        }
        catch (Exception e)
		{
			Debug.LogError("Set dictionary failed " + e.ToString());
		}
	}

	private void LoadFromDisk()
	{
		foreach (var bucket in m_buckets)
		{
			bucket.LoadFromDisk();
		}

		TransferAnyDataBetweenBuckets();
		RemoveOldKeys();
	}

	public void Save()
	{
		SaveToDisk();
		PlayerPrefs.Save();
	}

	private void SaveToDisk()
	{
		foreach (var bucket in m_buckets)
		{
			bucket.SaveToDisk();
		}
	}

	public void DoOnApplicationPause(bool pause)
	{
		if (pause)
		{
			Save();
		}
	}

	public void DoOnApplicationQuit()
	{
		Save();
	}

	public void DeleteKey(string key, string subKey)
	{
		BucketBase bucket = GetBucketForKey(key);
		string fullKey = !string.IsNullOrEmpty(subKey) ? key + subKey : key;
		bucket.DeleteKey(fullKey);
	}

	public void DeleteAll()
	{
		if (Application.isPlaying)
		{
			foreach (var bucket in m_buckets)
			{
				bucket.DeleteAll();
			}
		}
		else
		{
			PlayerPrefs.DeleteAll();
		}
	}

	private void RemoveOldKeys()
	{
		foreach (var bucket in m_buckets)
		{
			bucket.RemoveOldKeys(this);
		}
	}

	private void TransferAnyDataBetweenBuckets()
	{
		foreach (var bucket in m_buckets)
		{
			bucket.TransferAnyKeysToAnotherBucket(this);
		}
	}
}
