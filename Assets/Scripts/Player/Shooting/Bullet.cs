using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour, IDestroyable
{
    public event UnityAction<Bullet> OnBulletDestroyed;
    private Physics _physics;
    private Vector2 _velocity;
    private bool _isUsed;
    private BulletConfig _config;
    private float _maxSpeed;
    private float _thrust;
    private float _dragForce;

    private void OnEnable()
    {
        _physics.Velocity = Vector2.zero;
        _isUsed = false;
    }

    private void Awake()
    {
        var config = JsonConfigLoader.LoadFromResources<BulletConfig>("Configs/bullet_config");
        _config = config;
        _maxSpeed = config.MaxSpeed;
        _thrust = config.Thrust;
        _dragForce = config.DragForce;
        _physics = new Physics(_thrust, _dragForce, _maxSpeed, 0);
    } 

    private void Update()
    {
        _physics.AddAcceleration(transform.up);
        _velocity = _physics.UpdateForces(Time.deltaTime);
        transform.position += (Vector3)(_velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_isUsed) return;
        
        if (collision.TryGetComponent(out IDestroyable shootable))
        {
            _isUsed = true;
            shootable.Destroy(DestroyReason.Shootable);
            OnBulletDestroyed?.Invoke(this);
        }
    }

    public void Destroy(DestroyReason reason)
    {
        OnBulletDestroyed?.Invoke(this);
    }
}