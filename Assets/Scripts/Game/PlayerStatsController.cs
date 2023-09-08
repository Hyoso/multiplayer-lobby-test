using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsController : NetworkBehaviour
{
    [SerializeField] private ActiveTrinketSO m_defaultActiveTrinket;

    [SerializeField] private ActiveTrinketSO m_activeTrinket;
    [SerializeField] private List<PassiveTrinketSO> m_passiveTrinkets = new List<PassiveTrinketSO>();
   
    // cosmetics maybe come later?
    private List<CosmeticSO> m_cosmetics = new List<CosmeticSO>();

    private CharacterDisplay m_characterDisplay;

    private void Awake()
    {
        GameplayEvents.onActiveTrinketEquipped += GameplayEvents_onActiveTrinketEquipped;
        GameplayEvents.onPassiveTrinketEquipped += GameplayEvents_onPassiveTrinketEquipped;
    }

    private void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_characterDisplay = GetComponent<CharacterDisplay>();

        if (IsClient && IsOwner)
        {
            LoadTrinkets();
            LoadCosmetics();

            foreach (var item in m_passiveTrinkets)
            {
                UpdateCharacterDisplay(item.cosmetics);
            }
            UpdateCharacterDisplay(m_activeTrinket?.cosmetics);
            UpdateCharacterDisplay(m_cosmetics);
        }
    }

    public override void OnDestroy()
    {
        GameplayEvents.onActiveTrinketEquipped -= GameplayEvents_onActiveTrinketEquipped;
        GameplayEvents.onPassiveTrinketEquipped -= GameplayEvents_onPassiveTrinketEquipped;

        base.OnDestroy();
    }

    public void RegisterCharacterDisplay(CharacterDisplay characterDisplay)
    {
        m_characterDisplay = characterDisplay;
    }

    public void AddTrinket(ActiveTrinketSO trinket)
    {
        if (trinket == null) { return; }

        UnEquip(m_activeTrinket);
        Equip(trinket);

        UpdateCharacterDisplay(trinket.cosmetics);
        SaveTrinkets();
    }

    public void AddTrinket(PassiveTrinketSO trinket)
    {
        if (trinket == null) { return; }

        Equip(trinket);
        UpdateCharacterDisplay(trinket.cosmetics);
        SaveTrinkets();
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

        UpdateCharacterDisplay(new List<CosmeticSO>() { cosmetic });
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

    private void UpdateCharacterDisplay(List<CosmeticSO> cosmetics)
    {
        if (cosmetics == null) { return; }

        foreach (var item in cosmetics)
        {
            m_characterDisplay.UpdateCosmetic(item);
        }
    }

    private void GameplayEvents_onActiveTrinketEquipped(ActiveTrinketSO trinket)
    {
        if (IsClient && IsOwner)
        {
            AddTrinket(trinket);
        }
    }

    private void GameplayEvents_onPassiveTrinketEquipped(PassiveTrinketSO trinket)
    {
        if (IsClient && IsOwner)
        {
            AddTrinket(trinket);
        }
    }

    private void Equip(PassiveTrinketSO trinket)
    {
        // todo: update player character with new sprite
        if (!m_passiveTrinkets.Contains(trinket))
        {
            m_passiveTrinkets.Add(trinket);
        }
    }

    private void Equip(ActiveTrinketSO activeTrinket)
    {
        // todo: update player character with new sprite
        m_activeTrinket = activeTrinket;
    }

    private void Equip(CosmeticSO cosmetic)
    {
        // todo: update player character with new sprite
        
        m_cosmetics.Add(cosmetic);
    }

    private void UnEquip(PassiveTrinketSO trinket)
    {
        m_passiveTrinkets.Remove(trinket);
    }

    private void UnEquip(ActiveTrinketSO activeTrinket)
    {
        m_activeTrinket = null;
    }

    private void UnEquip(CosmeticSO cosmetic)
    {
        // todo: update player character with new sprite

        m_cosmetics.Remove(cosmetic);
    }

    private void LoadTrinkets()
    {
        m_passiveTrinkets = SaveSystem.Instance.GetList<PassiveTrinketSO>(BucketGameplay.PASSIVE_TRINKETS);
        m_activeTrinket = SaveSystem.Instance.GetScriptableObject<ActiveTrinketSO>(BucketGameplay.ACTIVE_TRINKET) as ActiveTrinketSO;
    }

    public void SaveTrinkets()
    {
        SaveSystem.Instance.SetList(BucketGameplay.PASSIVE_TRINKETS, "", m_passiveTrinkets);
        SaveSystem.Instance.SetScriptableObject(BucketGameplay.ACTIVE_TRINKET, m_activeTrinket);
    }

    private void LoadCosmetics()
    {
        m_cosmetics = SaveSystem.Instance.GetList<CosmeticSO>(BucketGameplay.EQUIPPED_COSMETICS);
    }
}   
