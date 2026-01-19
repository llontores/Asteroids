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
        _hazardSpawner.RewardableDied += AddScore;
    }
    
    public void AddScore(int score)
    {
        _reward += score;
        Debug.Log($"Reward: {_reward}");
    }

    public void Dispose()
    {
        _hazardSpawner.RewardableDied -= AddScore;
    }
}