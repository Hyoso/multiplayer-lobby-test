using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// Tested working with unity advertisements version 4.3.0
/// </summary>

#if ADS
public class AdsManager : Singleton<AdsManager>
{
	[SerializeField, ReadOnly] private AdProvider m_currentAdsProvider;
	[SerializeField] bool _testMode = true;

	protected override void Init()
    {
        GameObject adProvider = new GameObject("AdsProvider");
        adProvider.transform.SetParent(transform);
        m_currentAdsProvider = adProvider.AddComponent<UnityAdsProvider>();
        adProvider.gameObject.name = m_currentAdsProvider.GetAdProviderName();
    }

    private void Start()
    {
        InitializeAds();
        m_currentAdsProvider.LoadAds();
    }

    public void ShowIntAd()
	{
		m_currentAdsProvider.ShowInterstitialAd();
	}

	public void ShowRewardAd(System.Action onComplete, System.Action onFailed)
	{
		m_currentAdsProvider.ShowRewardAd(onComplete, onFailed);
	}

	public void ShowBannerAd()
	{
		m_currentAdsProvider.ShowBannerAd();
	}

	public void HideBannerAd()
	{
		m_currentAdsProvider.HideBannerAd();
	}

	public bool IntAdReady()
	{
		return m_currentAdsProvider.IntAdReady;
	}

	public bool RewardAdReady()
	{
		return m_currentAdsProvider.RewardAdReady;
	}

	public void InitializeAds()
	{
		m_currentAdsProvider.InitializeAds(_testMode);
	}
}
#endif