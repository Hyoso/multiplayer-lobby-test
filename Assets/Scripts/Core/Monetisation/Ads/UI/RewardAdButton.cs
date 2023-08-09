using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardAdButton : AdButton
{
#if ADS
	public override void ShowAd()
	{
		AdsManager.Instance.ShowRewardAd(
			onComplete: () => { }, 
			onFailed: () => { });
	}

	public override void HideAd()
	{
	}

	protected override void GameplayEvents_GenericAdEvent(int adEventId)
	{
		if (adEventId == (int)AdEventIds.REWARD_AD_LOADED)
		{
			m_button.interactable = true;
		}
	}
#endif
}
