using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ADS
public class GameplayAds : MonoBehaviour
{
    private void Awake()
    {
        // uncomment if this needs to check for ads on level loaded
        //GameplayEvents.OnLoadNextLevelEvent += GameplayEvents_OnLoadNextLevelEvent;
        GameplayEvents.GenericAdEvent += GameplayEvents_GenericAdEvent;
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        //GameplayEvents.OnLoadNextLevelEvent -= GameplayEvents_OnLoadNextLevelEvent;
        GameplayEvents.GenericAdEvent -= GameplayEvents_GenericAdEvent;
    }

    private void GameplayEvents_GenericAdEvent(int intValue)
    {
        if (intValue == (int)AdEventIds.BANNER_AD_LOADED)
        {
            AdsManager.Instance.ShowBannerAd();
        }
    }

    private void GameplayEvents_OnLoadNextLevelEvent()
    {
        StartCoroutine(Coroutines.Delay(0.75f, () => ShowAdCheck()));
    }

    private void ShowAdCheck()
    {
        int adCounter = SaveSystem.Instance.GetInt(BucketGameplay.AD_COUNTER);
        adCounter++;

        int nextAdCount = SaveSystem.Instance.GetInt(BucketGameplay.NEXT_AD_COUNT, "",
            Random.Range(GameplayConfig.Instance.minAdFrequency, GameplayConfig.Instance.maxAdFrequency));

        if (adCounter >= nextAdCount)
        {
            nextAdCount = Random.Range(GameplayConfig.Instance.minAdFrequency, GameplayConfig.Instance.maxAdFrequency);
            adCounter = 0;

            AdsManager.Instance.ShowIntAd();
        }

        SaveSystem.Instance.SetInt(BucketGameplay.AD_COUNTER, "", adCounter);
        SaveSystem.Instance.SetInt(BucketGameplay.NEXT_AD_COUNT, "", nextAdCount);
        SaveSystem.Instance.Save();
    }
}
#endif