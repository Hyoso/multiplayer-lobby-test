using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class CharacterDisplay : NetworkBehaviour
{
    private const float DIST_BETWEEN_HATS = 0.2f;

    [SerializeField] private GameObject m_netCosmetic;

    [Space]
    [SerializeField] private SpriteRenderer m_hat;
    [SerializeField] private SpriteRenderer m_feet;
    [SerializeField] private SpriteRenderer m_feather;
    [SerializeField] private SpriteRenderer m_body;
    [SerializeField] private SpriteRenderer m_trail;

    private float m_hatsCounter = 0f;

    private void Start()
    {
        PlayerStatsController statsController = GetComponent<PlayerStatsController>();
        statsController.RegisterCharacterDisplay(this);
    }

    public void UpdateCosmetic(CosmeticSO cosmetic)
    {
        switch (cosmetic.type) 
        {
            case CosmeticSO.CosmeticType.HAT: UpdateHat(cosmetic); break;
            case CosmeticSO.CosmeticType.FEET: UpdateFeet(cosmetic); break;
            case CosmeticSO.CosmeticType.FEATHER: UpdateFeather(cosmetic); break;
            case CosmeticSO.CosmeticType.BODY: UpdateBody(cosmetic); break;
            case CosmeticSO.CosmeticType.TRAIL: UpdateTrail(cosmetic); break;
            default: Debug.Log("No valid cosmetic type to update"); break;
        }
    }

    private void UpdateHat(CosmeticSO cosmetic)
    {
        CreateHatServerRpc(m_hat.GetComponentInParent<CharacterDisplay>(), cosmetic.equippedSprite.name);
    }

    private void UpdateFeet(CosmeticSO cosmetic)
    {
        ChangeSpriteClientRpc(m_feet.GetComponentInParent<CharacterDisplay>(), CosmeticSO.CosmeticType.FEET, cosmetic.equippedSprite.name);
    }

    private void UpdateFeather(CosmeticSO cosmetic)
    {
        ChangeSpriteClientRpc(m_feather.GetComponentInParent<CharacterDisplay>(), CosmeticSO.CosmeticType.FEATHER, cosmetic.equippedSprite.name);
    }

    private void UpdateBody(CosmeticSO cosmetic)
    {
        ChangeSpriteClientRpc(m_body.GetComponentInParent<CharacterDisplay>(), CosmeticSO.CosmeticType.BODY, cosmetic.equippedSprite.name);
    }

    private void UpdateTrail(CosmeticSO cosmetic)
    {
        //todo: change the trail renderer image to this sprite
        ChangeSpriteClientRpc(m_trail.GetComponentInParent<CharacterDisplay>(), CosmeticSO.CosmeticType.TRAIL, cosmetic.equippedSprite.name);
    }

    [ServerRpc]
    public void CreateHatServerRpc(NetworkBehaviourReference go, string newSpriteName, ServerRpcParams rpcParams = default)
    {
        if (go.TryGet<CharacterDisplay>(out CharacterDisplay charDisplay))
        {
            NetworkObject parentNetworkObject = charDisplay.GetComponentInParent<NetworkObject>(); ;

            SpriteRenderer sr = CreateSpriteRendererObjCopy(m_netCosmetic);
            sr.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId, true);
            sr.GetComponent<NetworkObject>().TrySetParent(parentNetworkObject);

            SetupHatSpriteClientRpc(sr.GetComponent<ClientNetworkTransform>(), newSpriteName);
            
            //charDisplay.m_hatsCounter++;

            //CreateHatClientRpc(go, newSpriteName);
        }

        //Debug.Log("Server doing something");

        //CreateHatClientRpc(go, newSpriteName);
    }

    [ClientRpc]
    public void SetupHatSpriteClientRpc(NetworkBehaviourReference newSpriteObject, string newSpriteName)
    {
        if (newSpriteObject.TryGet<ClientNetworkTransform>(out ClientNetworkTransform srTransform))
        {
            if (IsOwner)
            {
                SpriteRenderer sr = srTransform.GetComponent<SpriteRenderer>();
                NetworkCosmetic networkCosmetic = sr.GetComponent<NetworkCosmetic>();
                networkCosmetic.SetSpriteName(newSpriteName);

                Vector3 moveAmount = Vector3.up * DIST_BETWEEN_HATS * m_hatsCounter;
                Vector3 newPos = sr.transform.parent.position + moveAmount;
                networkCosmetic.Teleport(newPos);

                m_hatsCounter++;
            }
        }
    }

    [ClientRpc]
    public void CreateHatClientRpc(NetworkBehaviourReference go, string newSpriteName)
    {
        if (go.TryGet<CharacterDisplay>(out CharacterDisplay charDisplay))
        {
            SpriteRenderer sr = CreateSpriteRendererObjCopy(charDisplay.m_hat.gameObject);
            sr.transform.localPosition = Vector3.zero;
            sr.transform.Translate(Vector3.up * DIST_BETWEEN_HATS * m_hatsCounter);

            Sprite newSprite = CosmeticsManager.Instance.GetSprite(newSpriteName);
            sr.sprite = newSprite;

            charDisplay.m_hatsCounter++;
        }
    }

    [ServerRpc]
    public void ChangeSpriteServerRpc(NetworkBehaviourReference go, CosmeticSO.CosmeticType cosmeticType, string newSpriteName)
    {
        if (go.TryGet<CharacterDisplay>(out CharacterDisplay charDisplay))
        {
            SpriteRenderer sr = null;
            switch (cosmeticType)
            {
                case CosmeticSO.CosmeticType.FEET: sr = charDisplay.m_feet.GetComponent<SpriteRenderer>(); break;
                case CosmeticSO.CosmeticType.FEATHER: sr = charDisplay.m_feather.GetComponent<SpriteRenderer>(); break;
                case CosmeticSO.CosmeticType.BODY: sr = charDisplay.m_body.GetComponent<SpriteRenderer>(); break;
                case CosmeticSO.CosmeticType.TRAIL: sr = charDisplay.transform.GetComponent<SpriteRenderer>(); break;
            }

            Sprite newSprite = CosmeticsManager.Instance.GetSprite(newSpriteName);
            sr.sprite = newSprite;

            ChangeSpriteClientRpc(go, cosmeticType, newSpriteName);
        }
    }

    [ClientRpc]
    public void ChangeSpriteClientRpc(NetworkBehaviourReference go, CosmeticSO.CosmeticType cosmeticType, string newSpriteName)
    {
        // may need to check if this is not server before changing sprite/mkaing new hat
        Debug.Log("Client changing sprite");

        if (go.TryGet<CharacterDisplay>(out CharacterDisplay charDisplay))
        {
            SpriteRenderer sr = null;
            switch (cosmeticType)
            {
                case CosmeticSO.CosmeticType.FEET: sr = charDisplay.m_feet.GetComponent<SpriteRenderer>(); break;
                case CosmeticSO.CosmeticType.FEATHER: sr = charDisplay.m_feather.GetComponent<SpriteRenderer>(); break;
                case CosmeticSO.CosmeticType.BODY: sr = charDisplay.m_body.GetComponent<SpriteRenderer>(); break;
                case CosmeticSO.CosmeticType.TRAIL: sr = charDisplay.transform.GetComponent<SpriteRenderer>(); break;
            }

            Sprite newSprite = CosmeticsManager.Instance.GetSprite(newSpriteName);
            sr.sprite = newSprite;
        };
    }

    private SpriteRenderer CreateSpriteRendererObjCopy(GameObject go)
    {
        GameObject copy = Instantiate(go);
        SpriteRenderer sr = copy.GetComponent<SpriteRenderer>();

        return sr;
    }
}
