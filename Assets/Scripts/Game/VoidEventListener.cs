using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidEventListener : MonoBehaviour
{
	[SerializeField] private List<VoidEvent> m_voidEvents;

	private void Awake()
	{
		foreach (var item in m_voidEvents) 
		{
			item.voidEvent += GameplayEvents_VoidEvent;
		}
	}

	private void GameplayEvents_VoidEvent()
	{
		Debug.Log("hit");
	}
}
