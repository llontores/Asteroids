using UnityEngine;
using UnityEngine.Events;

public class UFO : Entity, IDestroyable
{
    private const int LeftTurnIndex = 1;
    private const int RightTurnIndex = -1;
    
    public event UnityAction<UFO> OnDestroy;
    
    private float _thrust;
    private float _drag;
    private float _maxSpeed;
    private float _spinningMinSpeed;
    private float _spinningMaxSpeed;
    private float _bounceForce;
    private int _spinningTurn;
    private float _spinningSpeed;
    private Vector2 _velocity;
    private Physics _physics;
    private Vector3 _direction;
    private Transform _target;
    private UFOConfig _config;
    
    private void Awake()
    {
        _config = JsonConfigLoader.LoadFromResources<UFOConfig>("Configs/ufo_config");
        _thrust = _config.Thrust;
        _drag = _config.Drag;
        _maxSpeed = _config.MaxSpeed;
        _spinningMinSpeed = _config.SpinningMinSpeed;
        _spinningMaxSpeed = _config.SpinningMaxSpeed;
        _bounceForce = _config.BounceForce;
        _reward = _config.Reward;
        _physics = new Physics(_thrust, _drag, _maxSpeed, _bounceForce);
        _spinningSpeed = Random.Range(_spinningMinSpeed, _spinningMaxSpeed + 1);
        _spinningTurn = Random.Range(RightTurnIndex, LeftTurnIndex + 1);
    }
    
    private void Update()
    {
        _direction = (_target.position - transform.position).normalized;
        _physics.AddAcceleration(_direction);
        _velocity = _physics.UpdateForces(Time.deltaTime);
        transform.position += (Vector3)(_velocity * Time.deltaTime);
        transform.Rotate(0, 0, _spinningTurn * Time.deltaTime * _spinningSpeed);
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

    public void InitTarget(Transform target)
    {
        _target = target;
    }
    
    public void Destroy(DestroyReason reason)
    {
        SetDestroyReason(reason);
        OnDestroy?.Invoke(this);
    }
}