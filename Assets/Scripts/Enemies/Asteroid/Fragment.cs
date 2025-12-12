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
        _physics = new Physics(_impulceForce, _dragForce, _maxSpeed);
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
}