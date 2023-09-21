using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();

            StartCoroutine(Coroutines.Delay(3f, () =>
            {
                GetComponent<NetworkObject>().Despawn(true);
            }));
        }
    }

    void Update()
    {
        transform.position += transform.right * Time.deltaTime;
    }
}
