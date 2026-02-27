using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Player : MonoBehaviour, IInitializable
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
    public void Construct(PlayerConfig config)
    {
        Config = config;
        _currentHealth =  Config.MaxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(IsInvulnerable)
            return;
        
        if(collision.TryGetComponent<Bullet>(out Bullet _))
            return;
            OnTriggerEntered?.Invoke(collision);
            _currentHealth = Mathf.Clamp(_currentHealth - 1, 0, Config.MaxHealth);
    }

    public void ChangeInvelnurabilityStatus(bool status)
    {
        IsInvulnerable = status;
    }

    public void Initialize()
    {
    }
}