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
    private Player _player;

    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _player = player;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<AccelerationSignal>(Accelerate);
        _signalBus.Subscribe<TurnSignal>(Turn);
        _player.OnTriggerEntered += Bounce;
        _turnSpeed = _player.Config.TurnSpeed;
        _playerTransform = _player.transform;
        _physics = new Physics(_player.Config.Thrust, _player.Config.DragForce, _player.Config.MaxSpeed, _player.Config.BounceForce);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<AccelerationSignal>(Accelerate);
        _signalBus.Unsubscribe<TurnSignal>(Turn);
        _player.OnTriggerEntered -= Bounce;
    }

    public void Tick()
    {
        Vector2 velocity = _physics.UpdateForces(Time.deltaTime);
        _playerTransform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void Accelerate()
    {
        if(_player.IsInvulnerable == true)
            return;
        
        _physics.AddAcceleration(_playerTransform.up);
    }

    private void Turn(TurnSignal args)
    {
        if(_player.IsInvulnerable == true)
            return;
     
        int turnIndex = args.TurnIndex;
        _playerTransform.Rotate(0, 0, turnIndex * _turnSpeed * Time.deltaTime);
    }

    private void Bounce(Collider2D other)
    {
        Vector2 contactPoint = other.ClosestPoint(_player.transform.position);
        Vector2 normal = ((Vector2)_player.transform.position - contactPoint).normalized;

        _physics.Bounce(normal);
    }
}