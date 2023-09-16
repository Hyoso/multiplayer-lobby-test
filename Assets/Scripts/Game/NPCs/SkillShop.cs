using NaughtyAttributes;
using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillShop : ShopBase
{
    [SerializeField] private TMPro.TextMeshPro m_levelText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetupNetworkSettings();
        UpdateUI();
    }

    [Command]
    public void IncrementLevel()
    {
        if (IsServer && IsOwner && IsClient)
        {
            shopLevel.Value++;

            SaveSystem.Instance.SetInt(BucketGameplay.TRINKET_SHOP_LEVEL, "", shopLevel.Value);
        }
    }

    private void UpdateUI()
    {
        m_levelText.text = shopLevel.Value.ToString();
    }

    private void UpdateLevelText(int prev, int cur)
    {
        m_levelText.text = shopLevel.Value.ToString();
    }

    private void SetupNetworkSettings()
    {
        shopLevel.OnValueChanged += UpdateLevelText;

        if (IsServer && IsOwner && IsClient)
        {
            shopLevel.Value = SaveSystem.Instance.GetInt(BucketGameplay.TRINKET_SHOP_LEVEL);
        }
    }
}
