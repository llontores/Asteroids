using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour, IShootable
{
    private const int LeftTurnIndex = 1;
    private const int RightTurnIndex = -1;
    
    [SerializeField] private float _thrust;
    [SerializeField] private float _drag;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _reward;
    [SerializeField] private float _spinningMinSpeed;
    [SerializeField] private float _spinningMaxSpeed;
    [SerializeField] private Fragment _fragmentPrefab;
    [SerializeField] private int _minFragmentAmount;
    [SerializeField] private int _maxFragmentAmount;
    
    public event UnityAction<Asteroid> OnDead;
    
    private int _spinningTurn;
    private float _spinningSpeed;
    private Vector2 _velocity;
    private Physics _physics;
    private Vector3 _direction;
    private ObjectPool<Fragment> _asteroidPool;
    
    private void Start()
    {
        _physics = new Physics(_thrust, _drag, _maxSpeed);
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
        transform.Rotate(0,0, _spinningTurn * Time.deltaTime * _spinningSpeed );
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    } 

    public void Die()
    {
        OnDead?.Invoke(this);
    }
}