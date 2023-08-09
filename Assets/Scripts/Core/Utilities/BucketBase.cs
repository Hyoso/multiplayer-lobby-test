using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BucketBase
{
	protected Hashtable m_data = new Hashtable();
	protected HashSet<string> m_keysToRemove;
	protected HashSet<string> m_keysToIgnoreOnServerLoad = new HashSet<string>();

	private bool m_needsSaveToDisk;

	private const string KEY_VERSION = "version";
	private const string KEY_DATA = "data";
	private const string KEY_COMPRESSED = "compressed";

	public virtual void AddKeys(Dictionary<string, BucketBase> keyBucketDict) { }
	public virtual void RemoveOldKeys(SaveSystem saveSystem) { }
	public virtual void TransferAnyKeysToAnotherBucket(SaveSystem saveSystem) { }
	public abstract string GetBucketKey();
	protected virtual void ProcessDataFromDisk() { }

	public Hashtable GetData()
	{
		return m_data;
	}

	public Hashtable GetCompressedData()
	{
		return BuildCompressedData();
	}

	public void DeleteKey(string key)
	{
		m_needsSaveToDisk = false;

		if (m_data.ContainsKey(key))
		{
			m_data.Remove(key);
			m_needsSaveToDisk = true;
		}
	}

	public void DeleteAll()
	{
		m_data.Clear();

		PlayerPrefs.DeleteKey(GetBucketKey());

		m_needsSaveToDisk = true;
	}

	public void SetValue(string key, object value)
	{
		if (m_data.ContainsKey(key))
		{
			m_data[key] = value;
		}
		else
		{
			m_data.Add(key, value);
		}

		m_needsSaveToDisk = true;
	}

	public object GetValue(string key, object defaultValue, ref bool hasKey)
	{
		if (m_data.ContainsKey(key))
		{
			hasKey = true;
			return m_data[key];
		}

		return defaultValue;
	}

	public void SaveToDisk()
	{
		if (m_needsSaveToDisk)
		{
			string jsonData = null;

			try
			{
				Hashtable data = BuildCompressedData();
				jsonData = JsonSerializer.JsonEncode(data);
			}
			catch (System.Exception e)
			{
				Debug.LogError("Save to disk failed " + e.ToString());

				jsonData = JsonSerializer.JsonEncode(m_data);
			}

			PlayerPrefs.SetString(GetBucketKey(), jsonData);
		}

		m_needsSaveToDisk = false;
	}

	public void LoadFromDisk()
	{
		string rawData = PlayerPrefs.GetString(GetBucketKey());

		if (!string.IsNullOrEmpty(rawData))
		{
			Hashtable data = JsonSerializer.JsonDecode(rawData) as Hashtable;
			m_data = BuildDecompressedData(data);
		}

		ProcessDataFromDisk();
	}

	private Hashtable BuildCompressedData()
	{
		string bucketDataJson = JsonSerializer.JsonEncode(m_data);
		string compressedJson = Compression.CompressStringGZip(bucketDataJson);

		Hashtable data = new Hashtable();
		data.Add(KEY_VERSION, CoreConfig.Instance.projectVersion);
		data.Add(KEY_COMPRESSED, true);
		data.Add(KEY_DATA, compressedJson);

		return data;
	}

	private Hashtable BuildDecompressedData(Hashtable data)
	{
		Hashtable output;

		if (data.ContainsKey(KEY_COMPRESSED) && (bool)data[KEY_COMPRESSED])
		{
			string compressedJson = data[KEY_DATA] as string;
			string decompressedJson = Compression.DecompressStringGZip(compressedJson);
			output = (Hashtable)JsonSerializer.JsonDecode(decompressedJson);
		}
		else
		{
			output = data;
		}

		return output;
	}
}
