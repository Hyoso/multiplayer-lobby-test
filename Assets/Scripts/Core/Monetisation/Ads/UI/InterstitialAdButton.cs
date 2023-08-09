using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterstitialAdButton : AdButton
{
#if ADS
	public override void ShowAd()
	{
		AdsManager.Instance.ShowIntAd();
	}

	public override void HideAd()
	{
	}

	protected override void GameplayEvents_GenericAdEvent(int adEventId)
	{
		if (adEventId == (int)AdEventIds.INT_AD_LOADED)
		{
			m_button.interactable = true;
		}
	}
#endif
}