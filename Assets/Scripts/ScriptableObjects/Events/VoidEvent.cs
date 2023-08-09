using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class VoidEvent : ScriptableObject
{
	public UnityAction voidEvent;

	public void SendEvent()
	{
		if (voidEvent != null)
		{
			voidEvent.Invoke();
		}
		else
		{
			Debug.Log("No registered events on " + this.name + " object");
		}
	}
}
