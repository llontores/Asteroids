using System;
using Signals;
using Zenject;
using UnityEngine;

public class PlayerMover : IInitializable, IDisposable, ITickable
{
    private SignalBus _signalBus;
    private float _turnSpeed;
    private Transform _playerTransform;
    private Physics _physics;

    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _signalBus = signalBus;
        _turnSpeed = player.TurnSpeed;
        _playerTransform = player.transform;
        _physics = new Physics(player.Thrust, player.DragForce, player.MaxSpeed);
    }

    public void Initialize()
    {
        _signalBus.Subscribe<AccelerationSignal>(Accelerate);
        _signalBus.Subscribe<TurnSignal>(Turn);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<AccelerationSignal>(Accelerate);
        _signalBus.Unsubscribe<TurnSignal>(Turn);
    }

    public void Tick()
    {
        Vector2 velocity = _physics.UpdateForces(Time.deltaTime);
        _playerTransform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void Accelerate()
    {
        _physics.AddAcceleration(_playerTransform.up);
    }

    private void Turn(TurnSignal args)
    {
        int turnIndex = args.TurnIndex;

        _playerTransform.Rotate(0, 0, turnIndex * _turnSpeed * Time.deltaTime);
    }
}