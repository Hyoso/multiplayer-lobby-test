using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkCosmetic))]
public class NetworkCosmeticInspector : MonoBehaviour
{
    [SerializeField, ReadOnly] private NetworkCosmetic m_networkCosmetic;
    [SerializeField] private string m_newCosmetic = "";

    private void Awake()
    {
        m_networkCosmetic = GetComponent<NetworkCosmetic>();
    }

    [Button]
    public void ChangeCosmetic()
    {
        m_networkCosmetic.ChangeSprite(m_newCosmetic);
    }

    [Button]
    public void Despawn()
    {
        m_networkCosmetic.Despawn();
    }
}
