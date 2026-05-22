using System;
using UnityEngine;
using Zenject;

public class RewardCounter : IDisposable, IInitializable
{
    private int _score;
    private SignalBus _signalBus;
    

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }
    
    public void TryAddScore(DestroyableDiedSignal args)
    {
        if(args.Entity.Reason == DestroyReason.World)
            return;
        
        _score += args.Entity.Reward;
        _signalBus.Fire(new ScoreChangedSignal{Score = _score});
    }

    public void Dispose()   
    {
        _signalBus.Unsubscribe<DestroyableDiedSignal>(TryAddScore);
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(ResetScore);
    }

    public void Initialize()
    {
        _signalBus.Subscribe<DestroyableDiedSignal>(TryAddScore);
        _signalBus.Subscribe<RestartButtonPressedSignal>(ResetScore);
        _score = 0;
        _signalBus.Fire(new ScoreChangedSignal{Score = _score});
    }

    private void ResetScore()
    {
        _score = 0;
        _signalBus.Fire(new ScoreChangedSignal{Score = _score});
    }
}