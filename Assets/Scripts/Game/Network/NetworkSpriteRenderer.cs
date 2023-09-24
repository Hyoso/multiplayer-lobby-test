using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NetworkSpriteRenderer : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> m_render = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<NetworkString> m_spriteName =
        new NetworkVariable<NetworkString>(new NetworkString { data = "" });

    [SerializeField, ReadOnly] private SpriteRenderer m_spriteRenderer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetupVariables();
    }

    private void OnValidate()
    {
        if (m_spriteRenderer == null)
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }
        else
        {
            m_render.Value = m_spriteRenderer.enabled;
            m_spriteName.Value = new NetworkString { data = m_spriteRenderer.sprite.name };
        }
    }

    public void TestChangeSprite()
    {
        Sprite sprite = SpriteAssetsConfig.Instance.GetSprite("Block_Sprite");
        ChangeSprite(sprite);
    }

    public void ChangeSprite(Sprite sprite)
    {
        if (IsOwner)
        {
            m_spriteRenderer.sprite = sprite;
            m_spriteName.Value = new NetworkString { data = sprite.name };
        }
    }

    public void ChangeRenderEnabled(bool renderEnabled)
    {
        m_render.Value = renderEnabled;
    }

    private void SetupVariables()
    {
        if (IsOwner)
        {
            m_render.Value = m_spriteRenderer.enabled;
            m_spriteName.Value = new NetworkString { data = m_spriteRenderer.sprite.name };
        }

        m_render.OnValueChanged = OnRenderValueChanged;
        m_spriteName.OnValueChanged = OnSpriteChanged;
    }

    private void OnSpriteChanged(NetworkString previousValue, NetworkString newValue)
    {
        Sprite sprite = SpriteAssetsConfig.Instance.GetSprite(newValue.data);
        m_spriteRenderer.sprite = sprite;
    }

    private void OnRenderValueChanged(bool previousValue, bool newValue)
    {
        m_spriteRenderer.enabled = newValue;
    }
}
