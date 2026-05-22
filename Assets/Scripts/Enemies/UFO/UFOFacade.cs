using System;
using UnityEngine;

public class UFOFacade : ITarget
{
    public event Action<UFOFacade> OnDead;

    public int Reward => _config.Reward;
    public Vector2 Position => _transform.position;
    public float ColliderRadius => _config.Radius;

    private UFOConfig _config;
    private UFOMover _mover;
    private UFORotator _rotator;
    private Transform _transform;
    private UFO _view;

    public UFOFacade(UFOConfig config)
    {
        _config = config;
    }

    public void Bind(UFO view)
    {
        _view = view;
        _transform = view.transform;
        _mover = new UFOMover(_config.Thrust, _config.Drag, _config.MaxSpeed, _config.BounceForce);
        _rotator = new UFORotator(_transform, _config.SpinningMinSpeed, _config.SpinningMaxSpeed);
    }

    public void SetTarget(Transform target)
    {
        _mover.SetTarget(target);
    }

    public void Update(float deltaTime)
    {
        _mover.Update(deltaTime, _transform);
        _rotator.Rotate(deltaTime);
    }

    public void Bounce(Collider2D collider2D)
    {
        Vector2 contactPoint = collider2D.ClosestPoint(_transform.position);
        Vector2 normal = ((Vector2)_transform.position - contactPoint).normalized;
        _mover.Bounce(normal);
    }

    public void ResetVelocity()
    {
        _mover.ResetVelocity();
    }

    public void Destroy(DestroyReason reason)
    {
        _view.SyncReason(reason);
        OnDead?.Invoke(this);
    }

    public UFO GetView()
    {
        return _view;
    }
}