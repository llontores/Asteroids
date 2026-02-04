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

    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _signalBus = signalBus;
        _animator = player.Animator;
        _bulletShootParticles = player.BulletShootParticles;
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
        _bulletShootParticles.Play();
    }

}