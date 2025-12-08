using System;
using Signals;
using UnityEngine;
using Zenject;

public class PlayerView : IInitializable, IDisposable
{
    private const string AcceleratingState = "IsAccelerating";

    private Animator _animator;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _signalBus = signalBus;
        _animator = player.Animator;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<AccelerationSignal>(ManageAccelerationAnimation);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<AccelerationSignal>(ManageAccelerationAnimation);
    }

    private void ManageAccelerationAnimation(AccelerationSignal args)
    {
        _animator.SetBool(AcceleratingState, args.Pressed);
    }

}