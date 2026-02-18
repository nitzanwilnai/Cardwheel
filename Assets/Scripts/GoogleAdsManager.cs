/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using CommonTools;
using UnityEngine;
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
using GoogleMobileAds.Api;
#endif

public class GoogleAdsManager : Singleton<GoogleAdsManager>
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

    float m_lastAdTime = -60.0f;
    float AD_COOLDOWN = 60.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize Google Mobile Ads Unity Plugin.
        MobileAds.Initialize(
            (InitializationStatus initStatus) => {
                // This callback is called once the MobileAds SDK is initialized.
            }
        );
    }

    public void LoadInterstitialAd()
    {
        Debug.Log("LoadInterstitiaAd()");
        if (m_insterstitialAd != null)
        {
            m_insterstitialAd.Destroy();
            m_insterstitialAd = null;
        }

        var adRequest = new AdRequest();

        // Send the request to load the ad.
        InterstitialAd.Load(
            AD_UNIT_ID,
            adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    // The ad failed to load.
                    return;
                }
                // The ad loaded successfully.
                m_insterstitialAd = ad;
            }
        );
    }

    public void ShowInterstitial()
    {
        Debug.Log("m_insterstitialAd != null " + (m_insterstitialAd != null));
        if (m_insterstitialAd != null)
            Debug.Log("m_insterstitialAd.CanShowAd() " + m_insterstitialAd.CanShowAd());

        if (Time.unscaledTime - m_lastAdTime >= AD_COOLDOWN)
        {
            if (m_insterstitialAd != null && m_insterstitialAd.CanShowAd())
            {
                Debug.Log("m_insterstitialAd.Show()");
                m_insterstitialAd.Show();

                m_lastAdTime = Time.unscaledTime;
            }
        }
    }
#endif
}
