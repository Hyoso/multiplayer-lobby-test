using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterDisplay : NetworkBehaviour
{
    private const float DIST_BETWEEN_HATS = 0.2f;

    [SerializeField] private SpriteRenderer m_hat;
    [SerializeField] private SpriteRenderer m_feet;
    [SerializeField] private SpriteRenderer m_feather;
    [SerializeField] private SpriteRenderer m_body;
    [SerializeField] private SpriteRenderer m_trail;

    private float m_hatHeight = 0f;

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
        ChangeSpriteServerRpc(m_feet.GetComponentInParent<CharacterDisplay>(), CosmeticSO.CosmeticType.FEET, cosmetic.equippedSprite.name);
    }

    private void UpdateFeather(CosmeticSO cosmetic)
    {
        ChangeSpriteServerRpc(m_feather.GetComponentInParent<CharacterDisplay>(), CosmeticSO.CosmeticType.FEATHER, cosmetic.equippedSprite.name);
    }

    private void UpdateBody(CosmeticSO cosmetic)
    {
        ChangeSpriteServerRpc(m_body.GetComponentInParent<CharacterDisplay>(), CosmeticSO.CosmeticType.BODY, cosmetic.equippedSprite.name);
    }

    private void UpdateTrail(CosmeticSO cosmetic)
    {
        //todo: change the trail renderer image to this sprite
        ChangeSpriteServerRpc(m_trail.GetComponentInParent<CharacterDisplay>(), CosmeticSO.CosmeticType.TRAIL, cosmetic.equippedSprite.name);
    }

    [ServerRpc]
    public void CreateHatServerRpc(NetworkBehaviourReference go, string newSpriteName)
    {
        if (go.TryGet<CharacterDisplay>(out CharacterDisplay charDisplay))
        {
            SpriteRenderer sr = CreateSpriteRendererObjCopy(m_hat.gameObject);
            sr.transform.Translate(Vector3.up * DIST_BETWEEN_HATS);

            Sprite newSprite = CosmeticsManager.Instance.GetSprite(newSpriteName);
            sr.sprite = newSprite;
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

            if (cosmeticType == CosmeticSO.CosmeticType.HAT)
            {
                sr = CreateSpriteRendererObjCopy(m_hat.gameObject);
                sr.transform.Translate(Vector3.up);
            }

            Sprite newSprite = CosmeticsManager.Instance.GetSprite(newSpriteName);
            sr.sprite = newSprite;
        }
    }

    private SpriteRenderer CreateSpriteRendererObjCopy(GameObject go)
    {
        GameObject copy = Instantiate(go);
        copy.transform.parent = go.transform.parent;

        SpriteRenderer sr = copy.GetComponent<SpriteRenderer>();

        return sr;
    }
}
