using System;
using GoogleMobileAds.Api;
using Zenject;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AdManager : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private InterstitialAd _interstitialAd;
    private const string AdUnitId = "ca-app-pub-3940256099942544/1033173712";

    public AdManager(SignalBus signalBus) => _signalBus = signalBus;

    public void Initialize()
    {
        Debug.unityLogger.logEnabled = false;
        MobileAds.RaiseAdEventsOnUnityMainThread = true; 
        MobileAds.Initialize(_ => LoadInterstitial());


        _signalBus.Subscribe<RestartButtonPressedSignal>(OnRestartRequested);
    }

    public void Dispose() => _signalBus.Unsubscribe<RestartButtonPressedSignal>(OnRestartRequested);

    private void OnRestartRequested()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.OnAdFullScreenContentClosed += () => RestartGame();
                _interstitialAd.Show();
        }
        else
        {
            RestartGame(); 
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        if (_interstitialAd != null) _interstitialAd.Destroy();
        InterstitialAd.Load(AdUnitId, new AdRequest(), (ad, err) => _interstitialAd = ad);
    }
}