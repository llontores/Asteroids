using UnityEngine;

public class UFOMover
{
    private readonly Physics _physics;
    private readonly Transform _transform;
    private Transform _target;
    private Vector2 _velocity;

    public UFOMover(Transform transform, UFOConfig config)
    {
        _transform = transform;
        _physics = new Physics(config.Thrust, config.Drag, config.MaxSpeed, config.BounceForce);
    }

    public void SetTarget(Transform target) => _target = target;

    public void Update(float deltaTime)
    {
        if (_target == null) return;

        Vector3 direction = (_target.position - _transform.position).normalized;
        _physics.AddAcceleration(direction);
        
        _velocity = _physics.UpdateForces(deltaTime);
        _transform.position += (Vector3)(_velocity * deltaTime);
    }

    public void Bounce(Vector2 normal) => _physics.Bounce(normal);
}