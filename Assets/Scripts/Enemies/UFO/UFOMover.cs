using UnityEngine;

public class UFOMover
{
    private readonly Physics _physics;
    private Transform _target;
    private Vector2 _velocity;
    
    public UFOMover(float thrust, float drag, float maxSpeed, float bounceForce)
    {
        _physics = new Physics(thrust, drag, maxSpeed, bounceForce);
    }

    public void SetTarget(Transform target) => _target = target;

    public void Update(float deltaTime, Transform transform)
    {
        if (_target == null) return;
        
        Vector3 direction = (_target.position - transform.position).normalized;
        _physics.AddAcceleration(direction);
        
        _velocity = _physics.UpdateForces(deltaTime);
        transform.position += (Vector3)(_velocity * deltaTime);
    }

    public void Bounce(Vector2 normal) => _physics.Bounce(normal);

    public void ResetVelocity()
    {
        _physics.ResetVelocity();
    }
}