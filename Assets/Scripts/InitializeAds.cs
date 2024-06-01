using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAds : MonoBehaviour, IUnityAdsInitializationListener
{

    [SerializeField] private string androidGameId;
    [SerializeField] private string iOsGameId;
    [SerializeField] private bool isTesting;

    private string gameId;

    private void Awake()
    {
        #if UNITY_ANDROID
            gameId = androidGameId;
        #elif UNITY_IOS
            gameId = iOsGameId;
        #endif

        if(!Advertisement.isInitialized)
            Advertisement.Initialize(gameId, isTesting, this);
    }

    public void OnInitializationComplete()
    {
        throw new System.NotImplementedException();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        throw new System.NotImplementedException();
    }
}



