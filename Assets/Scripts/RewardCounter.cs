using System;
using UnityEngine;
using Zenject;

public class RewardCounter : IDisposable
{
    private HazardSpawner _hazardSpawner;
    private int _reward;

    [Inject]
    public void Construct(HazardSpawner hazardSpawner)
    {
        _hazardSpawner = hazardSpawner;
        _hazardSpawner.RewardableDied += TryAddScore;
    }
    
    public void TryAddScore(int score, DestroyReason reason)
    {
        if(reason == DestroyReason.World)
            return;
        
        _reward += score;
        Debug.Log($"Reward: {_reward}");
    }

    public void Dispose()
    {
        _hazardSpawner.RewardableDied -= TryAddScore;
    }
}