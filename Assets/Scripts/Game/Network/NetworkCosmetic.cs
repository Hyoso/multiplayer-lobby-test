using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class NetworkCosmetic : NetworkBehaviour
{
    [System.Serializable]
    public struct CosmeticData : INetworkSerializable
    {
        public string cosmeticName;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref cosmeticName);
        }
    }

    [SerializeField]
    private NetworkVariable<CosmeticData> m_data = new NetworkVariable<CosmeticData>(
        new CosmeticData
        {
            cosmeticName = string.Empty
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private SpriteRenderer m_spriteRenderer;
    private ClientNetworkTransform m_netTransform;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_netTransform = GetComponent<ClientNetworkTransform>();
    }

    public void Teleport(Vector3 pos)
    {
        m_netTransform.Teleport(pos, Quaternion.identity, transform.localScale);
    }

    public void SetSpriteName(string spriteName)
    {
        if (IsOwner)
        {
            m_data.Value = new CosmeticData{ cosmeticName = spriteName };
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            m_data.OnValueChanged += UpdateCosmetic;
            UpdateCosmetic(m_data.Value, m_data.Value);
        }
    }

    private void UpdateCosmetic(CosmeticData previousValue, CosmeticData newValue)
    {
        if (IsClient)
        {
            Sprite newSprite = CosmeticsManager.Instance.GetSprite(newValue.cosmeticName);
            m_spriteRenderer.sprite = newSprite;
        }
    }
}
