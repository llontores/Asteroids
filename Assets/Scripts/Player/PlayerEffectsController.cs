using System;
using UnityEngine;
using Zenject;

public class PlayerEffectsController : IInitializable, IDisposable
{
    private static readonly int AcceleratingStateHash = Animator.StringToHash("IsAccelerating");
    
    private Animator _animator;
    private SignalBus _signalBus;
    private ParticleSystem _bulletShootParticles;
    private PlayerReferences _playerReferences;
    private PlayerFacade _playerFacade;

    [Inject]
    public void Construct(SignalBus signalBus, PlayerReferences playerReferences, PlayerFacade playerFacade)
    {
        _playerFacade = playerFacade;
        _playerReferences =  playerReferences;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<AccelerationSignal>(ManageAccelerationAnimation);
        _signalBus.Subscribe<BulletShootSignal>(EmitBulletShootParticles);
        _animator = _playerReferences.Animator;
        _bulletShootParticles = _playerReferences.BulletShootParticles;
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<AccelerationSignal>(ManageAccelerationAnimation);
        _signalBus.Unsubscribe<BulletShootSignal>(EmitBulletShootParticles);
    }

    private void ManageAccelerationAnimation(AccelerationSignal args)
    {
        _animator.SetBool(AcceleratingStateHash, args.Power > 0f);
    }

    private void EmitBulletShootParticles()
    {
        if (!_playerFacade.IsInvulnerable)
            _bulletShootParticles.Play();
    }
}