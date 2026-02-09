using System;
using Signals;
using UnityEngine;
using Zenject;

public class RewardCounter : IDisposable
{
    private int _score;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<DestroyableDiedSignal>(TryAddScore);
    }
    
    public void TryAddScore(DestroyableDiedSignal args)
    {
        if(args.Entity.Reason == DestroyReason.World)
            return;
        
        _score += args.Entity.Reward;
        Debug.Log($"Score: {_score}");
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<DestroyableDiedSignal>(TryAddScore);
    }
}