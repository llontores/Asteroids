using System;
using UnityEngine;
using Zenject;
using UniRx;
using MVVM;

public class PlayerViewModel : IInitializable, IDisposable, ITickable
{
    private const int RoundValue = 2;
    private Player _player;
    private Transform _playerTransform;
    private SignalBus _signalBus;
    private PlayerFacade _playerFacade;

    [Data("Score")]
    public readonly ReactiveProperty<string> Score = new("0");
    
    [Data("LaserAmmo")]
    public readonly ReactiveProperty<string> LaserAmmo = new("0");
    
    [Data("LaserCooldown")]
    public readonly ReactiveProperty<string> LaserCooldown = new("0");
    
    [Data("XAxis")]
    public readonly ReactiveProperty<string> XAxis = new("0");
    
    [Data("YAxis")]
    public readonly ReactiveProperty<string> YAxis = new("0");
    
    [Data("ZRotation")]
    public readonly ReactiveProperty<string> ZRotation = new("0");
    
    [Data("Speed")]
    public readonly ReactiveProperty<string> Speed = new("0");
    
    [Data("Health")]
    public readonly ReactiveProperty<string> Health = new("0");
    
    [Inject]
    public void Construct(Player player, SignalBus signalBus, PlayerFacade playerFacade)
    {
        _player = player;
        _signalBus = signalBus;
        _playerTransform = _player.transform;
        _playerFacade = playerFacade;
    }
    
    public void Tick()
    {
        XAxis.Value =  "X: " + Math.Round(_playerTransform.position.x, RoundValue);
        YAxis.Value = "Y: " + Math.Round(_playerTransform.position.y, RoundValue);
        ZRotation.Value = "Z: " + Math.Round(_playerTransform.eulerAngles.z, RoundValue);
        Health.Value = $"HP: {_playerFacade.CurrentHealth}/{_playerFacade.MaxHealth}";
    }

    public void Initialize()
    {
        _signalBus.Subscribe<PlayerSpeedChangedSignal>(UpdateSpeed);
        _signalBus.Subscribe<ScoreChangedSignal>(UpdateScore);
        _signalBus.Subscribe<LaserReloadRemainingTimeChangedSignal>(UpdateReloadRemainTime);
        _signalBus.Subscribe<LaserRemainingAmmoCountUpdatedSignal>(UpdateRemainAmmo);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<PlayerSpeedChangedSignal>(UpdateSpeed);
        _signalBus.Unsubscribe<ScoreChangedSignal>(UpdateScore);
        _signalBus.Unsubscribe<LaserReloadRemainingTimeChangedSignal>(UpdateReloadRemainTime);
        _signalBus.Unsubscribe<LaserRemainingAmmoCountUpdatedSignal>(UpdateRemainAmmo);
        
        Score.Dispose();
        LaserAmmo.Dispose();
        LaserCooldown.Dispose();
        XAxis.Dispose();
        YAxis.Dispose();
        ZRotation.Dispose();
        Speed.Dispose();
    }

    private void UpdateRemainAmmo(LaserRemainingAmmoCountUpdatedSignal args)
    {
        LaserAmmo.Value = "Laser Ammo: " + args.AmmoCount;
    }

    private void UpdateReloadRemainTime(LaserReloadRemainingTimeChangedSignal args)
    {
        LaserCooldown.Value = "Laser Cooldown: " + Math.Round(args.RemainingTime, RoundValue);
    }

    private void UpdateScore(ScoreChangedSignal args)
    {
        Score.Value = "Score: " + args.Score;
    }

    private void UpdateSpeed(PlayerSpeedChangedSignal args)
    {
        Speed.Value = "Speed: " + Math.Round(args.CurrentSpeed, RoundValue);
    }
}