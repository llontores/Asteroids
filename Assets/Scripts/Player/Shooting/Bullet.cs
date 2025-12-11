using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _thrust;
    [SerializeField] private float _dragForce;

    public event UnityAction<Bullet> OnBulletDestroyed;
    
    private Physics _physics;
    private Vector2 _velocity;

    private void OnEnable()
    {
        _physics.Velocity = Vector2.zero;
    }

    private void Awake()
    {
        _physics = new Physics(_thrust, _dragForce, _maxSpeed);
    } 

    private void Update()
    {
        _physics.AddAcceleration(transform.up);
        _velocity = _physics.UpdateForces(Time.deltaTime);
        transform.position += (Vector3)(_velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IShootable shootable))
        {
            shootable.Die();
        }
    }

    public void Destroy()
    {
        OnBulletDestroyed?.Invoke(this);
    }
}