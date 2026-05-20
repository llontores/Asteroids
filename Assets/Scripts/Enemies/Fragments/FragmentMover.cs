using UnityEngine;

public class FragmentMover
{
    private readonly Physics _physics;
    private Vector2 _velocity;

    public FragmentMover(float impulseForce, float dragForce, float maxSpeed, float bounceForce)
    {
        _physics = new Physics(impulseForce, dragForce, maxSpeed, bounceForce);
    }

    public void OnFragmentEnable(Vector3 direction)
    {
        _physics.AddAcceleration(direction);
    }

    public void Update(float deltaTime, Transform transform)
    {
        _velocity = _physics.UpdateForces(deltaTime);
        transform.position += (Vector3)(_velocity * deltaTime);
    }

    public void Bounce(Vector2 normal)
    {
        _physics.Bounce(normal);
    }

    public void ResetVelocity()
    {
        _physics.ResetVelocity();
    }
}