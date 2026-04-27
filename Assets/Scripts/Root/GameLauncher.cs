using System;
using UnityEngine;
using Zenject;

public class GameLauncher : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly GameOverScreen _gameOverScreen;

    public GameLauncher(SignalBus signalBus, GameOverScreen gameOverScreen)
    {
        _signalBus = signalBus;
        _gameOverScreen = gameOverScreen;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<PlayerDeadSignal>(PauseGameAndShowUI);
        _gameOverScreen.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<PlayerDeadSignal>(PauseGameAndShowUI);
    }

    private void PauseGameAndShowUI()
    {
        Time.timeScale = 0;
        _gameOverScreen.gameObject.SetActive(true);
    }
}