using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterDisplay : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer m_hat;
    [SerializeField] private SpriteRenderer m_feet;
    [SerializeField] private SpriteRenderer m_feather;
    [SerializeField] private SpriteRenderer m_body;
    [SerializeField] private SpriteRenderer m_trail;

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
        SpriteRenderer sr = CreateSpriteRendererObjCopy(m_hat.gameObject);
        sr.sprite = cosmetic.equippedSprite;
    }

    private void UpdateFeet(CosmeticSO cosmetic)
    {
        SpriteRenderer sr = CreateSpriteRendererObjCopy(m_feet.gameObject);
        sr.sprite = cosmetic.equippedSprite;
    }

    private void UpdateFeather(CosmeticSO cosmetic)
    {
        SpriteRenderer sr = CreateSpriteRendererObjCopy(m_feather.gameObject);
        sr.sprite = cosmetic.equippedSprite;
    }

    private void UpdateBody(CosmeticSO cosmetic)
    {
        SpriteRenderer sr = CreateSpriteRendererObjCopy(m_body.gameObject);
        sr.sprite = cosmetic.equippedSprite;
    }

    private void UpdateTrail(CosmeticSO cosmetic)
    {
        //todo: change the trail renderer image to this sprite
        SpriteRenderer sr = CreateSpriteRendererObjCopy(m_trail.gameObject);
        sr.sprite = cosmetic.equippedSprite;
    }

    private SpriteRenderer CreateSpriteRendererObjCopy(GameObject go)
    {
        GameObject copy = Instantiate(go);
        copy.transform.parent = go.transform.parent;

        SpriteRenderer sr = copy.GetComponent<SpriteRenderer>();

        return sr;
    }
}
