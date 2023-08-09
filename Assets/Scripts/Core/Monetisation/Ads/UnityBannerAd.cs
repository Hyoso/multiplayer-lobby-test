using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class UnityBannerAd : MonoBehaviour
{
#if ADS
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms.

    //[SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        // Set the banner position:
        //Advertisement.Banner.SetPosition(_bannerPosition);
    }

    public void LoadAd()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnLoaded,
            errorCallback = OnError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId, options);
    }

    // Implement code to execute when the loadCallback event triggers:
    void OnLoaded()
    {
        Debug.Log("Banner loaded");
		GameplayEvents.SendGenericAdEvent((int)AdEventIds.BANNER_AD_LOADED);
    }

    // Implement code to execute when the load errorCallback event triggers:
    void OnError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }

    // Implement a method to call when the Show Banner button is clicked:
    public void ShowAd()
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(_adUnitId);
    }

    // Implement a method to call when the Hide Banner button is clicked:
    public void HideAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
    }

    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }

    void OnDestroy()
    {
    }
#endif
}
