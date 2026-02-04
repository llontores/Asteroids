using System;
using Signals;
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
        _animator = _player.Animator;
        _bulletShootParticles = _player.BulletShootParticles;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<AccelerationSignal>(ManageAccelerationAnimation);
        _signalBus.Subscribe<BulletShootSignal>(EmitBulletShootParticles);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<AccelerationSignal>(ManageAccelerationAnimation);
        _signalBus.Unsubscribe<BulletShootSignal>(EmitBulletShootParticles);
    }

    private void ManageAccelerationAnimation(AccelerationSignal args)
    {
        _animator.SetBool(AcceleratingState, args.IsPressed);
    }

    private void EmitBulletShootParticles()
    {
        if (!_player.IsInvulnerable)
            _bulletShootParticles.Play();
    }
}