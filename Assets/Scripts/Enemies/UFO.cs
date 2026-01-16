using UnityEngine;
using UnityEngine.Events;

public class UFO : MonoBehaviour, IDestroyable
{
    private const int LeftTurnIndex = 1;
    private const int RightTurnIndex = -1;

    [SerializeField] private float _thrust;
    [SerializeField] private float _drag;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _reward;
    [SerializeField] private float _spinningMinSpeed;
    [SerializeField] private float _spinningMaxSpeed;
    [SerializeField] private float _bounceForce;
    
    public event UnityAction<UFO> OnDestroy;
    private int _spinningTurn;
    private float _spinningSpeed;
    private Vector2 _velocity;
    private Physics _physics;
    private Vector3 _direction;
    private Transform _target;
    
    private void Start()
    {
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
        if (other.TryGetComponent(out Player player))
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
        OnDestroy?.Invoke(this);
    }
}