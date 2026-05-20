using System;
using UnityEngine;
using UnityEngine.Events;

public class FragmentFacade 
{
    private readonly FragmentConfig _config;
    private FragmentMover _mover;
    private Transform _transform;

    public int Reward => _config.Reward;

    public FragmentFacade(FragmentConfig config)
    {
        _config = config;
    }

    public void Init(Transform transform)
    {
        _transform = transform;
        
        _mover = new FragmentMover(_config.ImpulseForce, _config.DragForce, _config.MaxSpeed, _config.BounceForce);
    }

    public void OnFragmentEnable()
    {
        _transform.Rotate(0, 0, UnityEngine.Random.Range(0, _config.MaxDegree + 1));
        _mover.OnFragmentEnable(_transform.up); 
    }

    public void Update(float deltaTime)
    {
        _mover.Update(deltaTime, _transform);
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

}