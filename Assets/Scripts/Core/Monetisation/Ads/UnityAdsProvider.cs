using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

#if ADS
public class UnityAdsProvider : AdProvider, IUnityAdsInitializationListener
{
	[SerializeField] string _iOSGameId = "5370160"; // get these from ad provider
	[SerializeField] string _androidGameId = "5370161"; // get these from ad provider
	private string _gameId = "";

	private UnityInterstitialAd m_interstitialAd;
	private UnityRewardAd m_rewardAd;
	private UnityBannerAd m_bannerAd;

	private void Awake()
	{
		m_interstitialAd = gameObject.AddComponent<UnityInterstitialAd>();
		m_rewardAd = gameObject.AddComponent<UnityRewardAd>();
		m_bannerAd = gameObject.AddComponent<UnityBannerAd>();

		GameplayEvents.GenericAdEvent += GameplayEvents_GenericAdEvent;
	}

	private void OnDestroy()
	{
		GameplayEvents.GenericAdEvent -= GameplayEvents_GenericAdEvent;
	}

	private void GameplayEvents_GenericAdEvent(int adEventId)
	{
		switch (adEventId)
		{
			case (int)AdEventIds.REWARD_AD_LOADED:
				RewardAdReady = true;
				break;
			case (int)AdEventIds.INT_AD_LOADED:
				IntAdReady = true;
				break;
			case (int)AdEventIds.BANNER_AD_LOADED:
				BannerAdReady = true;
				break;
		}
	}

	public override void ShowInterstitialAd()
	{
		m_interstitialAd.ShowAd();
	}

	public override void ShowRewardAd(Action onComplete, Action onFailed)
	{
		m_rewardAd.ShowAd();
	}

	public override void ShowBannerAd()
	{
		m_bannerAd.ShowAd();
	}

	public override void HideBannerAd()
	{
		m_bannerAd.HideAd();
	}

	public override void LoadAds()
	{
		m_rewardAd.LoadAd();
		m_interstitialAd.LoadAd();
		m_bannerAd.LoadAd();
	}

	public override void InitializeAds(bool testMode)
	{
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
		_gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
		if (!Advertisement.isInitialized && Advertisement.isSupported)
		{
			Advertisement.Initialize(_gameId, testMode, this);
		}
	}

	public void OnInitializationComplete()
	{
		Debug.Log("Unity Ads initialization complete.");
	}

	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
	}

	public override string GetAdProviderName()
	{
		return "UnityAdsProvider";
	}
}
#endif