using System;
using UnityEngine;
using UnityEngine.Events;

public class AsteroidFacade
{
    public int Reward => _config.Reward;

    private AsteroidMover _mover;
    private AsteroidRotator _rotator;
    private AsteroidConfig _config;
    private Transform _transform;
    private AsteroidFragmentsSpawner _fragsSpawner;
    
    public AsteroidFacade(AsteroidConfig config)
    {
        _config = config;
    }
    
    public void Init(Transform transform)
    {
        _transform = transform;
        _rotator = new AsteroidRotator(_transform, _config.SpinningMinSpeed, _config.SpinningMaxSpeed);
        _mover = new AsteroidMover(_config.Thrust, _config.Drag, _config.MaxSpeed, _config.BounceForce);
        _fragsSpawner = new AsteroidFragmentsSpawner(_config);
    }

    public void InitPool(FragmentsPool pool)
    {
        _fragsSpawner.Init(pool, _transform);
    }

    public void OnAsteroidEnable()
    {
        _mover.ResetVelocity();
    }

    public void Update(float deltaTime)
    {
        _mover.Update(deltaTime, _mover.Velocity, _transform );
        _rotator.Speen(deltaTime);
    }

    public void SetDirection(Vector3 direction)
    {
        _mover.SetDirection(direction);
    }

    public void Bounce(Collider2D collider2D)
    {
        Vector2 contactPoint = collider2D.ClosestPoint(_transform.position);
        Vector2 normal = ((Vector2)_transform.position - contactPoint).normalized;
        
        _mover.Bounce(normal);
    }

    public void Destroy(DestroyReason reason)
    {
        if (reason != DestroyReason.World)
        {
            _fragsSpawner.SpawnFragments();
        }
    }

}