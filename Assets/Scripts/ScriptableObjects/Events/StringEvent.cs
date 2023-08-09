using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class StringEvent : ScriptableObject
{
	public UnityAction<string> stringEvent;

	public void SendEvent(string str)
	{
		if (stringEvent != null)
		{
			stringEvent.Invoke(str);
		}
		else
		{
			Debug.Log("No registered events on " + this.name + " object");
		}
	}
}
