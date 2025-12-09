using System;
using DefaultNamespace;
using Signals;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class Asteroid : MonoBehaviour, IShootable
{
    [SerializeField] private float _thrust;
    [SerializeField] private float _drag;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _reward;
    [SerializeField] private float _speeningTurn;
    
    private Vector2 _velocity;
    private Physics _physics;
    private SignalBus _signalBus;

    private void Start()
    {
        _physics = new Physics(_thrust, _drag, _maxSpeed);
    }

    private void OnEnable()
    {
        _velocity = Vector2.zero;
    }
    

    public void Update()
    {
        _physics.AddAcceleration(transform.up);
        _velocity = _physics.UpdateForces(Time.deltaTime);
        transform.position += (Vector3)(_velocity * Time.deltaTime);
    }

    public void Die()
    {
        _signalBus.Fire(new AsteroidDeadSignal(){Asteroid = this});
    }
}