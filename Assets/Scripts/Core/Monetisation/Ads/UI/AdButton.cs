using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AdButton : MonoBehaviour
{
#if ADS
	[SerializeField] protected Button m_button; 
	public abstract void ShowAd();
	public abstract void HideAd();
	protected abstract void GameplayEvents_GenericAdEvent(int adEventId);
	

	private void Awake()
	{
		GameplayEvents.GenericAdEvent += GameplayEvents_GenericAdEvent;
	}

	private void OnDestroy()
	{
		GameplayEvents.GenericAdEvent -= GameplayEvents_GenericAdEvent;
	}
#endif
}
