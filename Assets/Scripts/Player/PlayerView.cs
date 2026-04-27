using System;
using UnityEngine;
using Zenject;

public class PlayerView : IInitializable, IDisposable
{
    private const string AcceleratingState = "IsAccelerating";

    private Animator _animator;
    private SignalBus _signalBus;
    private ParticleSystem _bulletShootParticles;
    private Player _player;

    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _player = player;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<AccelerationSignal>(ManageAccelerationAnimation);
        _signalBus.Subscribe<BulletShootSignal>(EmitBulletShootParticles);
        _animator = _player.Animator;
        _bulletShootParticles = _player.BulletShootParticles;
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<AccelerationSignal>(ManageAccelerationAnimation);
        _signalBus.Unsubscribe<BulletShootSignal>(EmitBulletShootParticles);
    }

    private void ManageAccelerationAnimation(AccelerationSignal args)
    {
        _animator.SetBool(AcceleratingState, args.Power > 0f);
    }

    private void EmitBulletShootParticles()
    {
        if (!_player.IsInvulnerable)
            _bulletShootParticles.Play();
    }
}