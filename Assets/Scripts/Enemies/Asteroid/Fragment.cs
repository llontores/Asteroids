using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Fragment : Entity, IDestroyable
{
    private const int MaxDegree = 360;
    
    public event UnityAction<Fragment> OnDestroy;
    
    private float _impulceForce;
    private float _dragForce;
    private float _maxSpeed;
    private float _bounceForce;
    private Physics _physics;
    private Vector2 _velocity;
    private FragmentConfig _config;

    private void OnEnable()
    {
        transform.Rotate(0,0,Random.Range(0,MaxDegree + 1));
        _physics.AddAcceleration(transform.up);
    }

    private void Awake()
    {
        _config = JsonConfigLoader.LoadFromResources<FragmentConfig>("Configs/fragment_config");
        _impulceForce = _config.ImpulseForce;
        _dragForce = _config.DragForce;
        _maxSpeed = _config.MaxSpeed;
        _bounceForce = _config.BounceForce;
        _reward = _config.Reward;
        _physics = new Physics(_impulceForce, _dragForce, _maxSpeed, _bounceForce);
    }

    private void Update()
    {
        _velocity = _physics.UpdateForces(Time.deltaTime);
        transform.position += (Vector3)(_velocity * Time.deltaTime);
    }

    public void Destroy(DestroyReason reason)
    {
        SetDestroyReason(reason);
        OnDestroy?.Invoke(this);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player ) || other.TryGetComponent(out InvulnerableCircle invulnerableCircle))

        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            Vector2 normal = ((Vector2)transform.position - contactPoint).normalized;

            _physics.Bounce(normal);
        }
    }
}