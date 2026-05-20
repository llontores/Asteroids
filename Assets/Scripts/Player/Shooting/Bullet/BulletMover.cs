using UnityEngine;

public class BulletMover
{
    private Physics _physics;
    private Vector2 _velocity;

    public BulletMover(float thrust, float dragForce, float maxSpeed)
    {
        _physics = new Physics(thrust, dragForce, maxSpeed, 0);
    }

    public void ResetVelocity()
    {
        _velocity = Vector2.zero;
        
        if (_physics != null)
        {
            _physics.Velocity = Vector2.zero;
        }
    }

    public void Update(float deltaTime, Transform transform)
    {
        _physics.AddAcceleration(transform.up);
        _velocity = _physics.UpdateForces(deltaTime);
        transform.position += (Vector3)(_velocity * deltaTime);
    }
}