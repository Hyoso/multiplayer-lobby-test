using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class for any ad provider
/// derived classes are initialised in adsManager class
/// </summary>
public abstract class AdProvider : MonoBehaviour
{
	public bool IntAdReady;
	public bool RewardAdReady;
	public bool BannerAdReady;

	public abstract void ShowBannerAd();
	public abstract void HideBannerAd();
	public abstract void ShowInterstitialAd();
	public abstract void ShowRewardAd(System.Action onComplete, System.Action onFailed);
	public abstract void LoadAds();
	public abstract void InitializeAds(bool testMode);
	public abstract string GetAdProviderName();
}
