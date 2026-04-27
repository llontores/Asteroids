using Firebase;
using Firebase.Analytics;
using Zenject;
using System;

public class AnalyticsManager : IInitializable
{
    private readonly SignalBus _signalBus;
    private bool _isInitialized = false;

    public AnalyticsManager(SignalBus signalBus)
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

    private void SubscribeToEvents()
    {
        _signalBus.Subscribe<PlayerDeadSignal>(() => {
            LogEvent("player_death");
        });

        _signalBus.Subscribe<RestartButtonPressedSignal>(() => {
            LogEvent("restart_clicked");
        });
    }

    private void LogEvent(string eventName)
    {
        if (!_isInitialized) return;
        FirebaseAnalytics.LogEvent(eventName);
    }
}