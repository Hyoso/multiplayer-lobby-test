using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBaseSO : ScriptableObject
{
	protected GameObject m_characterObject;

    public abstract void UpdateMovement();

    protected virtual void Awake()
	{
		Debug.Log("Awake " + name);
    }
    public virtual void Init()
	{

	}

    protected virtual void OnDestroy()
	{
		Debug.Log("Destroy " + name);
	}
	
	// must assign in start up
	public void SetCharacterObject(GameObject go)
	{
		m_characterObject = go;
    }
}
