using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StringEventListener : MonoBehaviour
{
	[SerializeField] private List<StringEvent> m_events;

	private void Awake()
	{
		foreach (var item in m_events)
		{
			item.stringEvent += GameplayEvents_StringEvent;
		}
	}

	private void GameplayEvents_StringEvent(string str)
	{
		Debug.Log(str);
	}
}
