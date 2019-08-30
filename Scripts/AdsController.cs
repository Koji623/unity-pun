using UnityEngine;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour, IUnityAdsListener
{

#if UNITY_IOS
    private string gameId = "3269403";
#elif UNITY_ANDROID
    private string gameId = "3269402";
#endif

    public bool testMode = true;

    public string rewardedPlacementId = "video";

    public string bannerPlacementId = "bannerPlacement";


    void Start()
    {
        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode);
        }
    }

    public void LoadBanner()
    {
        if (!Advertisement.Banner.isLoaded)
        {
            BannerLoadOptions loadOptions = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };
            Advertisement.Banner.Load(bannerPlacementId, loadOptions);
        }
    }
    void OnBannerLoaded()
    {
        Debug.Log("Banner Loaded");
        Advertisement.Banner.Show(bannerPlacementId);
    }
    void OnBannerError(string error)
    {
        Debug.Log("Banner Error: " + error);
    }

    // Implement a function for showing a rewarded video ad:
    public void ShowVideo()
    {
        if (Advertisement.IsReady(rewardedPlacementId))
        {
            Advertisement.Show(rewardedPlacementId);
        }
    }


    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == bannerPlacementId)
        {
            LoadBanner();
        }
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == rewardedPlacementId)
        {

        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
}