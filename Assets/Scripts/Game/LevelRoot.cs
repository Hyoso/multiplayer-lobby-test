using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoot : MonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.RegisterLevelRoot(this);
		GameplayEvents.SendLevelLoadedEvent();
	}
}
