using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRewardAdButton : MonoBehaviour
{
#if ADS
	[SerializeField] private Button m_button;

	private void Awake()
	{
		m_button.interactable = AdsManager.Instance.RewardAdReady();
		GameplayEvents.GenericAdEvent += GameplayEvents_GenericAdEvent;
	}

	private void OnDestroy()
	{
		GameplayEvents.GenericAdEvent -= GameplayEvents_GenericAdEvent;
	}

	private void GameplayEvents_GenericAdEvent(int adEventId)
	{
		if (adEventId == (int)AdEventIds.REWARD_AD_LOADED)
		{
			m_button.interactable = true;
		}
	}

	public void ShowAd()
	{
		AdsManager.Instance.ShowRewardAd(null, null);
	}
#endif
}
