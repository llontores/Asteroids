using UnityEngine;

public class AsteroidMover
{
    public Vector2 Velocity => _velocity;

    private Physics _physics;
    private Vector3 _direction;
    private Vector2 _velocity;

    public AsteroidMover(float thrust, float drag, float maxSpeed, float bounceForce)
    {
        _physics = new Physics(thrust, drag, maxSpeed, bounceForce);
    }

    public void ResetVelocity()
    {
        _velocity = Vector2.zero;
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }

    public void UpdateForces(float deltaTime)
    {
        _physics.AddAcceleration(_direction);
        _velocity = _physics.UpdateForces(deltaTime);
    }

    public void Bounce(Vector2 normal)
    {
        _physics.Bounce(normal);
    }
}