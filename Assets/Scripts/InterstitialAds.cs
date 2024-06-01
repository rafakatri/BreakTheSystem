using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iOsAdUnitId;

    private string adUnitId;
    private string nextScreen;

    private void Awake()
    {
        #if UNITY_ANDROID
            adUnitId = androidAdUnitId;
        #elif UNITY_IOS
            adUnitId = iOsAdUnitId;
        #endif
    }

    public void LoadInterstitialAd()
    {
        Advertisement.Load(adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {

    }

    public void OnInitializationComplete()
    {

    }

    public void OnInitializationFailed(UnityAdsLoadError error, string message)
    {

    }

    public void OnUnityAdsAdFailedToLoad(string message)
    {

    }

    public void ShowInterstitialAd(string screen_text)
    {
        nextScreen = screen_text;
        Advertisement.Show(adUnitId, this);
        LoadInterstitialAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {

    }

    public void OnUnityAdsShowClick(string placementId)
    {

    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        SceneManager.LoadScene(nextScreen);
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        SceneManager.LoadScene(nextScreen);
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        SceneManager.LoadScene(nextScreen);
    }



}
