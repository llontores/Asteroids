using Firebase;
using Firebase.Analytics;
using Zenject;
using System;

public class AnalyticsTracker : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private bool _isInitialized = false;

    public AnalyticsTracker(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                _isInitialized = true;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);
                
                SubscribeToEvents();
            }
            else
            {
                UnityEngine.Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }
    

    public void Dispose()
    {
        _signalBus.Unsubscribe<PlayerDeadSignal>(OnPlayerDead);
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(OnRestartClicked);
    }

    private void SubscribeToEvents()
    {
        _signalBus.Subscribe<PlayerDeadSignal>(OnPlayerDead);
        _signalBus.Subscribe<RestartButtonPressedSignal>(OnRestartClicked);
    }
    
    private void OnPlayerDead()
    {
        LogEvent("player_death");
    }

    private void OnRestartClicked()
    {
        LogEvent("restart_clicked");
    }

    private void LogEvent(string eventName)
    {
        if (!_isInitialized) return;
        FirebaseAnalytics.LogEvent(eventName);
    }

}