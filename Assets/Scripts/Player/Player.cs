using DefaultNamespace;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _thrust;
    [SerializeField] private float _dragForce;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private Animator _animator;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private int _bulletsShootCooldown;
    [SerializeField] private Bullet  _bulletPrefab;
    
    public float TurnSpeed => _turnSpeed;
    public float Thrust => _thrust;
    public float DragForce => _dragForce;
    public float MaxSpeed => _maxSpeed;
    public Animator Animator => _animator;
    public Renderer Renderer => _renderer;
    public Transform ShootPoint => _shootPoint;
    public int BulletsShootCooldown => _bulletsShootCooldown;
    public Bullet BulletPrefab => _bulletPrefab;
}