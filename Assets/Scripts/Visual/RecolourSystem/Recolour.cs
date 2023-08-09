using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recolour : MonoBehaviour
{
	public enum ObjectType
	{
		EXAMPLE_PARAM
    }

    [SerializeField] private ObjectType m_objectType;

	private void Awake()
	{
		RecolourObject();
		GameplayEvents.LevelLoadedEvent += GameplayEvents_LevelLoadedEvent;
	}

	private void OnDestroy()
	{
		GameplayEvents.LevelLoadedEvent -= GameplayEvents_LevelLoadedEvent;
	}

	private void GameplayEvents_LevelLoadedEvent()
	{
		RecolourObject();
	}

	private void RecolourObject()
	{
		switch (m_objectType)
		{
			case ObjectType.EXAMPLE_PARAM:
				RecolourExample();
				break;
        }
	}

	private void RecolourExample()
	{
    }
}
