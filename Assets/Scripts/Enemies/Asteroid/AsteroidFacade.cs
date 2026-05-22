using System;
using UnityEngine;

public class AsteroidFacade : ITarget
{
    public event Action<AsteroidFacade> OnDead;

    public int Reward => _config.Reward;
    public Vector2 Position => _transform.position;
    public float ColliderRadius => _config.Radius; 

    private AsteroidConfig _config;
    private FragmentsPool _fragmentsPool;
    private Transform _transform;
    private Asteroid _view;

    private AsteroidMover _mover;
    private AsteroidRotator _rotator;
    private AsteroidFragmentsSpawner _fragsSpawner;
    
    public AsteroidFacade(AsteroidConfig config, FragmentsPool fragmentsPool)
    {
        _config = config;
        _fragmentsPool = fragmentsPool;
    }
    
    public void Bind(Asteroid view)
    {
        _view = view;
        _transform = view.transform;
        
        _rotator = new AsteroidRotator(_transform, _config.SpinningMinSpeed, _config.SpinningMaxSpeed);
        _mover = new AsteroidMover(_config.Thrust, _config.Drag, _config.MaxSpeed, _config.BounceForce);
        
        _fragsSpawner = new AsteroidFragmentsSpawner(_config);
        _fragsSpawner.Init(_fragmentsPool, _transform);
    }

    public void OnAsteroidEnable()
    {
        _mover.ResetVelocity();
    }

    public void Update(float deltaTime)
    {
        _mover.Update(deltaTime, _mover.Velocity, _transform);
        _rotator.Spin(deltaTime);
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
        _view.SyncReason(reason);
    
        if (reason == DestroyReason.Shootable)
        {
            _fragsSpawner.SpawnFragments();
        }
        
        OnDead?.Invoke(this); 
    }
    public Asteroid GetView()
    {
        return _view;
    }
}