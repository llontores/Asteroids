using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Player : MonoBehaviour, IWrappable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LayerMask _layerMaskIgnore;
    [SerializeField] private PolygonCollider2D _polygonCollider2D;
    [SerializeField] private ParticleSystem _bulletShootParticles;
    [SerializeField] private InvulnerableCircle _invulnerableEffectCircle;

    private int _currentHealth;
    private SignalBus _signalBus;

    public Transform Transform => transform;
    public PlayerConfig Config { get; private set; }
    public event UnityAction<Collider2D> OnTriggerEntered;
    public Animator Animator => _animator;
    public Renderer Renderer => _renderer;
    public Transform ShootPoint => _shootPoint;
    public Bullet BulletPrefab => _bulletPrefab;
    public LineRenderer LineRenderer => _lineRenderer;
    public LayerMask LayerMaskIgnore => _layerMaskIgnore;
    public PolygonCollider2D PolygonCollider2D => _polygonCollider2D;
    public bool IsInvulnerable { get; private set; }

    public ParticleSystem BulletShootParticles => _bulletShootParticles;
    public InvulnerableCircle InvulnerableEffectCircle => _invulnerableEffectCircle;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Awake()
    {
        Config = JsonConfigLoader.LoadFromResources<PlayerConfig>("Configs/player_config");
        _currentHealth = Config.MaxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInvulnerable)
            return;

        if (collision.TryGetComponent<Bullet>(out Bullet _))
            return;

        OnTriggerEntered?.Invoke(collision);
        _currentHealth = Mathf.Clamp(_currentHealth - 1, 0, Config.MaxHealth);

        if (_currentHealth <= 0)
            _signalBus.Fire(new PlayerDeadSignal());
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<RestartButtonPressedSignal>(ResetHealth);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(ResetHealth);
    }

    public void ChangeInvelnurabilityStatus(bool status)
        {
            IsInvulnerable = status;
        }
    
    private void ResetHealth()
    {
        _currentHealth = Config.MaxHealth;
    }
}