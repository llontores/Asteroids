using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
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
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _laserDuration;
    [SerializeField] private float _maxRayDistance;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LayerMask _layerMaskIgnore;
    [SerializeField] private float _laserCooldown;
    [SerializeField] private float _maxLaserAmmoCount;
    [SerializeField] private float _laserReloadCooldown;
    [SerializeField] private float _bounceForce;
    [SerializeField] private PolygonCollider2D _polygonCollider2D;
    [SerializeField] private int _invelnurabilityDuration;
    [SerializeField] private int _invulnerabilityCoolDown;
    [SerializeField] private int _maxHealth;
    [SerializeField] private ParticleSystem _bulletShootParticles;
    [SerializeField] private InvulnerableCircle _invulnerableEffectCircle;
    
    private int _currentHealth;

    public event UnityAction<Collider2D> OnTriggerEntered;
    public float TurnSpeed => _turnSpeed;
    public float Thrust => _thrust;
    public float DragForce => _dragForce;
    public float MaxSpeed => _maxSpeed;
    public Animator Animator => _animator;
    public Renderer Renderer => _renderer;
    public Transform ShootPoint => _shootPoint;
    public int BulletsShootCooldown => _bulletsShootCooldown;
    public Bullet BulletPrefab => _bulletPrefab;
    public float LaserDuration => _laserDuration;
    public float MaxRayDistance => _maxRayDistance;
    public LineRenderer LineRenderer => _lineRenderer;
    public LayerMask LayerMaskIgnore => _layerMaskIgnore;
    public float LaserCooldown => _laserCooldown;
    public float MaxLaserAmmoCount => _maxLaserAmmoCount;
    public float LaserReloadCooldown => _laserReloadCooldown;
    public float BounceForce => _bounceForce;
    public PolygonCollider2D PolygonCollider2D => _polygonCollider2D;
    public int InvulnerabilityDuration => _invelnurabilityDuration;
    public int InvulnerabilityCoolDown => _invulnerabilityCoolDown;
    public bool IsInvulnerable { get; private set; }

    public ParticleSystem BulletShootParticles => _bulletShootParticles;
    public InvulnerableCircle InvulnerableEffectCircle => _invulnerableEffectCircle;    

    private void Start()
    {
        _currentHealth =  _maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(IsInvulnerable)
            return;
        
        if(collision.TryGetComponent<Bullet>(out Bullet _))
            return;

            Debug.Log($"коллизия сработала {collision.gameObject.name}" );
            OnTriggerEntered?.Invoke(collision);
            _currentHealth = Mathf.Clamp(_currentHealth - 1, 0, _maxHealth);
        
    }

    public void ChangeInvelnurabilityStatus(bool status)
    {
        IsInvulnerable = status;
    }
}