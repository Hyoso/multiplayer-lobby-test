using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TargetBase : NetworkBehaviour
{
    public  Vector3 position { get { return transform.position; } private set { } }

    private void Start()
    {
        TargetsManager.Instance.RegisterTarget(this);
    }

    public override void OnNetworkSpawn()
    {
    }
}
