using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsController : NetworkBehaviour
{
    [SerializeField] private TrinketSO m_activeTrinket;
    [SerializeField] private List<TrinketSO> m_passiveTrinkets = new List<TrinketSO>();
   
    // cosmetics maybe come later?
    private List<CosmeticSO> m_cosmetics = new List<CosmeticSO>();

    private CharacterDisplay m_characterDisplay;

    private void Awake()
    {
        GameplayEvents.onTrinketEquipped += GameplayEvents_onTrinketEquipped;
    }

    private void Start()
    {
        if (IsLocalPlayer)
        {
            LoadSkills();
            LoadCosmetics();
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.onTrinketEquipped -= GameplayEvents_onTrinketEquipped;
    }

    public void RegisterCharacterDisplay(CharacterDisplay characterDisplay)
    {
        m_characterDisplay = characterDisplay;
    }

    public void AddItem(TrinketSO trinket)
    {
        if (trinket == null) { return; }

        if (trinket.GetTrinketType() == TrinketSO.TrinketType.ACTIVE)
        {
            UnEquip(trinket);
            Equip(trinket);
        }
        else if (trinket.GetTrinketType() == TrinketSO.TrinketType.PASSIVE)
        {
            Equip(trinket);
        }
    }

    public void AddItem(CosmeticSO cosmetic)
    {
        if (cosmetic == null) { return; }

        if (m_cosmetics.Find(x => x.type == cosmetic.type))
        {
            foreach (var item in m_cosmetics)
            {
                if (item.type == cosmetic.type)
                {
                    UnEquip(item);
                    Equip(cosmetic);
                    break;
                }
            }
        }
        else
        { 
            Equip(cosmetic);
        }
    }

    public float GetSkillValue(PlayerStats.StatType type)
    {
        float total = 0;
        foreach (var item in m_passiveTrinkets)
        {
            total += item.boostToPlayerStats.GetStat(type);
        }

        return total;
    }

    private void GameplayEvents_onTrinketEquipped(TrinketSO trinket)
    {
        if (IsLocalPlayer)
        {
            Equip(trinket);
        }
    }

    private void Equip(TrinketSO trinket)
    {
        // todo: update player character with new sprite

        if (trinket.GetTrinketType() == TrinketSO.TrinketType.ACTIVE)
        {
            m_activeTrinket = trinket;
        }
        else if (trinket.GetTrinketType() == TrinketSO.TrinketType.PASSIVE)
        {
            m_passiveTrinkets.Add(trinket);
        }
    }

    private void Equip(CosmeticSO cosmetic)
    {
        // todo: update player character with new sprite
        
        m_cosmetics.Add(cosmetic);
    }

    private void UnEquip(TrinketSO trinket)
    {
        // todo: update player character with new sprite

        if (trinket.GetTrinketType() == TrinketSO.TrinketType.ACTIVE)
        {
            m_activeTrinket = null;
        }
        else if (trinket.GetTrinketType() == TrinketSO.TrinketType.PASSIVE)
        {
            m_passiveTrinkets.Remove(trinket);
        }
    }

    private void UnEquip(CosmeticSO cosmetic)
    {
        // todo: update player character with new sprite

        m_cosmetics.Remove(cosmetic);
    }

    private void LoadSkills()
    {
        m_passiveTrinkets = SaveSystem.Instance.GetList<TrinketSO>(BucketGameplay.EQUIPPED_TRINKETS);
    }

    private void LoadCosmetics()
    {
        m_cosmetics = SaveSystem.Instance.GetList<CosmeticSO>(BucketGameplay.EQUIPPED_COSMETICS);
    }
}   
