using UnityEngine;
using UnityEngine.Events;

public class Asteroid : Entity, IDestroyable
{
    private const int LeftTurnIndex = 1;
    private const int RightTurnIndex = -1;

    [SerializeField] private float _thrust;
    [SerializeField] private float _drag;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _spinningMinSpeed;
    [SerializeField] private float _spinningMaxSpeed;
    [SerializeField] private int _minFragmentAmount;
    [SerializeField] private int _maxFragmentAmount;
    [SerializeField] private float _bounceForce;

    public event UnityAction<Asteroid> OnDead;


    private int _spinningTurn;
    private float _spinningSpeed;
    private Vector2 _velocity;
    private Physics _physics;
    private Vector3 _direction;
    private FragmentsPool _fragmentsPool;
    private int _fragmentsAmount;
    private Fragment _spawnedFragment;

    private void Start()
    {
        _physics = new Physics(_thrust, _drag, _maxSpeed, _bounceForce);
        _spinningSpeed = Random.Range(_spinningMinSpeed, _spinningMaxSpeed + 1);
        _spinningTurn = Random.Range(RightTurnIndex, LeftTurnIndex + 1);
    }

    private void OnEnable()
    {
        _velocity = Vector2.zero;
    }

    public void Update()
    {
        _physics.AddAcceleration(_direction);
        _velocity = _physics.UpdateForces(Time.deltaTime);
        transform.position += (Vector3)(_velocity * Time.deltaTime);
        transform.Rotate(0, 0, _spinningTurn * Time.deltaTime * _spinningSpeed);
    }

    public void Init(FragmentsPool pool)
    {
        _fragmentsPool = pool;
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }

    public void Destroy(DestroyReason reason)
    {
        SetDestroyReason(reason);
        
        if (reason != DestroyReason.World)
        {
            SpawnFragments();
        }

        OnDead?.Invoke(this);
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

    private void SpawnFragments()
    {
        _fragmentsAmount = Random.Range(_minFragmentAmount, _maxFragmentAmount + 1);

        for (int i = 0; i < _fragmentsAmount; i++)
        {
            _spawnedFragment = _fragmentsPool.GetFragment();
            _spawnedFragment.transform.position = transform.position;
            _spawnedFragment.gameObject.SetActive(true);
        }
    }
}