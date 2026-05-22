using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private PolygonCollider2D _polygonCollider2D;
    [SerializeField] private ParticleSystem _bulletShootParticles;
    [SerializeField] private InvulnerableCircle _invulnerableEffectCircle;

    public Animator Animator => _animator;
    public Renderer Renderer => _renderer;
    public Transform ShootPoint => _shootPoint;
    public Bullet BulletPrefab => _bulletPrefab;
    public LineRenderer LineRenderer => _lineRenderer;
    public PolygonCollider2D PolygonCollider2D => _polygonCollider2D;
    public ParticleSystem BulletShootParticles => _bulletShootParticles;
    public InvulnerableCircle InvulnerableEffectCircle => _invulnerableEffectCircle;
}