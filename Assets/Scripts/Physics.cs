using UnityEngine;

public class Physics
{
    public Vector2 Velocity;
    private Vector2 _acceleration;
    private float _thrust;
    private float _dragForce;
    private float _maxSpeed;

    public Physics(float thrust, float dragForce, float maxSpeed)
    {
        _thrust = thrust;
        _dragForce = dragForce;
        _maxSpeed = maxSpeed;
    }

    public Vector2 UpdateForces(float deltaTime)
    {
        Velocity += _acceleration * deltaTime;
        Velocity *= 1 / (1 + _dragForce * deltaTime);

        if (Velocity.sqrMagnitude > _maxSpeed * _maxSpeed)
        {
            Velocity = Velocity.normalized * _maxSpeed;
        }

        _acceleration = Vector2.zero;
        
        return Velocity;
    }

    public void AddAcceleration(Vector2 forward)
    {

        _acceleration += forward *  _thrust;
    }
    
    public void Bounce(Vector2 collidedObjectPosition, float bounciness = 0.8f)
    {
        collidedObjectPosition = collidedObjectPosition.normalized;
        Velocity = Velocity - 2f * Vector2.Dot(Velocity, collidedObjectPosition) * collidedObjectPosition;
        Velocity *= bounciness;
    }
}