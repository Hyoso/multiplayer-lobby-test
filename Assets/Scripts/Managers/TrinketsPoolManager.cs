using NaughtyAttributes;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrinketsPoolManager : Singleton<TrinketsPoolManager>
{
    #region TESTS

    public PassiveTrinketSO passiveTrinket;
    public ActiveTrinketSO activeTrinket;

    protected override void Init()
    {
    }

    [Button, Command]
    public void EquipActive()
    {
        GameplayEvents.SendOnActiveTrinketEquippedEvent(activeTrinket);
    }


    [Button, Command]
    public void EquipPassive()
    {
        GameplayEvents.SendOnPassiveTrinketEquippedEvent(passiveTrinket);
    }
    #endregion

    public struct TrinketSet
    {
        public ActiveTrinketSO activeTrinket;
        public PassiveTrinketSO passiveTrinket1;
        public PassiveTrinketSO passiveTrinket2;
        public PassiveTrinketSO passiveTrinket3;
    }

    [SerializeField] private List<ActiveTrinketSO> m_allActiveTrinkets = new List<ActiveTrinketSO>();
    [SerializeField] private List<PassiveTrinketSO> m_allPassiveTrinkets = new List<PassiveTrinketSO>();
    private List<ActiveTrinketSO> m_unlockedActiveTrinkets = new List<ActiveTrinketSO>();
    private List<PassiveTrinketSO> m_unlockedPassiveTrinkets = new List<PassiveTrinketSO>();

    private void Start()
    {
        PopulateUnlockedTrinkets();
    }

    public TrinketSet GetTrinketSet()
    {
        List<PassiveTrinketSO> passiveTrinketsCopy = new List<PassiveTrinketSO>(m_unlockedPassiveTrinkets.Count);

        TrinketSet output = new TrinketSet()
        {
            activeTrinket = GetRandomActiveTrinket(),
            passiveTrinket1 = passiveTrinketsCopy.GetRandomElementAndRemove(),
            passiveTrinket2 = passiveTrinketsCopy.GetRandomElementAndRemove(),
            passiveTrinket3 = passiveTrinketsCopy.GetRandomElementAndRemove(),
        };

        return output;
    }

    public ActiveTrinketSO GetRandomActiveTrinket()
    {
        ActiveTrinketSO trinket = m_unlockedActiveTrinkets.GetRandom();
        return trinket;
    }

    public PassiveTrinketSO GetRandomPassiveTrinket()
    {
        PassiveTrinketSO trinket = m_unlockedPassiveTrinkets.GetRandom();
        return trinket;
    }

    public PassiveTrinketSO GetPassiveTrinketWithName(string trinketName)
    {
        PassiveTrinketSO trinket = m_allPassiveTrinkets.Find(x => x.displayName == trinketName);
        return trinket;
    }

    private void PopulateUnlockedTrinkets()
    {

        foreach (var item in m_allActiveTrinkets)
        {
            if (item.IsUnlocked())
            {
                m_unlockedActiveTrinkets.Add(item);
            }
        }

        foreach (var item in m_allPassiveTrinkets)
        {
            if (item.IsUnlocked())
            {
                m_unlockedPassiveTrinkets.Add(item);
            }
        }
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        SearchTrinkets<ActiveTrinketSO>("Active", ref m_allActiveTrinkets);
        SearchTrinkets<PassiveTrinketSO>("Passive", ref m_allPassiveTrinkets);
#endif
    }

    private void SearchTrinkets<T>(string folderName, ref List<T> trinketsList) where T : ScriptableObject
    {
#if UNITY_EDITOR
        string folderPath = "Assets/Data/Trinkets/";

        string activeTrinketsPath = folderPath + folderName;
        List<T> activeTrinkets = ScriptableObjectUtils.LoadScriptableObjectsFromFolder<T>(activeTrinketsPath);

        foreach (T obj in activeTrinkets)
        {
            if (!trinketsList.Contains(obj))
            {
                trinketsList.Add(obj);
            }
        }
#endif
    }
}
