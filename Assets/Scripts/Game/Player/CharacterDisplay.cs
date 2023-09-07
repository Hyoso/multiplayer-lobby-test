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
        m_hat.sprite = cosmetic.equippedSprite;
    }

    private void UpdateFeet(CosmeticSO cosmetic)
    {
        m_feet.sprite = cosmetic.equippedSprite;
    }

    private void UpdateFeather(CosmeticSO cosmetic)
    {
        m_feather.sprite = cosmetic.equippedSprite;
    }

    private void UpdateBody(CosmeticSO cosmetic)
    {
        m_body.sprite = cosmetic.equippedSprite;
    }

    private void UpdateTrail(CosmeticSO cosmetic)
    {
        //todo: change the trail renderer image to this sprite
        m_trail.sprite = cosmetic.equippedSprite;
    }
}
