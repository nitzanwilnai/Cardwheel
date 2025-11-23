using UnityEngine;
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
using GoogleMobileAds.Api;
#endif

public class GoogleAdsManager : MonoBehaviour
{
#if UNITY_ANDROID
        private const string AD_UNIT_ID = "ca-app-pub-2715576489475489/3665452846";
#elif UNITY_IPHONE
        private const string AD_UNIT_ID = "ca-app-pub-2715576489475489/4104452452";
#else
    private const string AD_UNIT_ID = "unused";
#endif

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
    InterstitialAd m_insterstitialAd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize Google Mobile Ads Unity Plugin.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });
    }

    public void LoadInterstitialAd()
    {
        if (m_insterstitialAd != null)
        {
            m_insterstitialAd.Destroy();
            m_insterstitialAd = null;
        }

        var adRequest = new AdRequest();

        // Send the request to load the ad.
        InterstitialAd.Load(AD_UNIT_ID, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                // The ad failed to load.
                return;
            }
            // The ad loaded successfully.
            m_insterstitialAd = ad;
        });
    }

    public void ShowInterstitial()
    {
        if (m_insterstitialAd != null && m_insterstitialAd.CanShowAd())
        {
            m_insterstitialAd.Show();
        }
    }
#endif
}
