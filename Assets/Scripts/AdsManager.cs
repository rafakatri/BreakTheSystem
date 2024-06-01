using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager: MonoBehaviour
{
    public InitializeAds initializeAds;
    public InterstitialAds interstitialAds;

    public static AdsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        interstitialAds.LoadInterstitialAd();
    }

}