using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Fragment : MonoBehaviour, IDestroyable
{
    private const int MaxDegree = 360;
    
    [SerializeField] private float _impulceForce;
    [SerializeField] private float _dragForce;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _bounceForce;
    
    public event UnityAction<Fragment> OnDestroy;
    
    private Physics _physics;
    private Vector2 _velocity;

    private void OnEnable()
    {
        transform.Rotate(0,0,Random.Range(0,MaxDegree + 1));
        _physics.AddAcceleration(transform.up);
    }

    private void Awake()
    {
        _physics = new Physics(_impulceForce, _dragForce, _maxSpeed, _bounceForce);
    }

    private void Update()
    {
        _velocity = _physics.UpdateForces(Time.deltaTime);
        transform.position += (Vector3)(_velocity * Time.deltaTime);
    }

    public void Destroy(DestroyReason reason)
    {
        OnDestroy?.Invoke(this);
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
}