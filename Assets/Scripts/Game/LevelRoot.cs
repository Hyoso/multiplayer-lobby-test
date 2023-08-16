using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelRoot : NetworkBehaviour
{
	private void Awake()
	{
		GameManager.Instance.RegisterLevelRoot(this);
		GameplayEvents.SendLevelLoadedEvent();
	}

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

		Debug.Log("Despawned level root");
    }
}
