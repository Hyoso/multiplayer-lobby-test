using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBaseSO : ScriptableObject
{
	public abstract void UpdateMovement(GameObject go);

	protected virtual void Awake()
	{
		Debug.Log("Awake " + name);
	}

	protected virtual void OnDestroy()
	{
		Debug.Log("Destroy " + name);
	}
}
