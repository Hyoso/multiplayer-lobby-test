using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerAdButton : AdButton
{
#if ADS
	protected override void GameplayEvents_GenericAdEvent(int adEventId)
	{
		if (adEventId == (int)AdEventIds.INT_AD_LOADED)
		{
			m_button.interactable = true;
		}
	}

	public override void HideAd()
	{
		AdsManager.Instance.HideBannerAd();
	}

	public override void ShowAd()
	{
		AdsManager.Instance.ShowBannerAd();
	}
#endif
}
