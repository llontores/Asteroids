using UnityEngine;

public class FragmentMover
{
    private readonly Physics _physics;
    private readonly Transform _transform;
    private Vector2 _velocity;

    public FragmentMover(Transform transform, FragmentConfig config)
    {
        _transform = transform;
        _physics = new Physics(config.ImpulseForce, config.DragForce, config.MaxSpeed, config.BounceForce);
    }

    public void OnEnable()
    {
        _physics.AddAcceleration(_transform.up);
    }

    public void Update(float deltaTime)
    {
        _velocity = _physics.UpdateForces(deltaTime);
        _transform.position += (Vector3)(_velocity * deltaTime);
    }

    public void Bounce(Vector2 normal)
    {
        _physics.Bounce(normal);
    }
}