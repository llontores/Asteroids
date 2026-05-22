using System;
using UnityEngine;

public class FragmentFacade : ITarget
{
    public event Action<FragmentFacade> OnDead;

    public int Reward => _config.Reward;
    public Vector2 Position => _transform.position;
    public float ColliderRadius => _config.Radius; 

    private readonly FragmentConfig _config;
    private FragmentMover _mover;
    private Transform _transform;
    private Fragment _view;

    public FragmentFacade(FragmentConfig config)
    {
        _config = config;
    }

    public void Bind(Fragment view)
    {
        _view = view;
        _transform = view.transform;
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

        public void Destroy(DestroyReason reason)
    {
        _view.SyncReason(reason);
        OnDead?.Invoke(this); 
    }

    public Fragment GetView()
    {
        return _view;
    }
}