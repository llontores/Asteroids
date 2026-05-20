using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Asteroid : Entity, ITarget
{
    private AsteroidFacade _facade;
    private SpriteRenderer _spriteRenderer;
    public event UnityAction<Asteroid> OnDead;
    public Vector2 Position => transform.position;
    public float ColliderRadius { get; private set; }

    [Inject]
    public void Construct(AsteroidFacade facade)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _facade = facade;
        _facade.Init(transform);
        _reward = _facade.Reward;
        ColliderRadius = Mathf.Max(_spriteRenderer.bounds.extents.x, _spriteRenderer.bounds.extents.y);
    }

    private void OnEnable()
    {
        _facade?.OnAsteroidEnable();
    }

    private void Update()
    {
        _facade?.Update(Time.deltaTime);
    }

    public void Init(FragmentsPool pool)
    {
        _facade.InitPool(pool);
    }

    public void SetDirection(Vector3 direction)
    {
        _facade.SetDirection(direction);
    }

    public void Destroy(DestroyReason reason)
    {
        SetDestroyReason(reason);
        _facade.Destroy(reason);
        OnDead?.Invoke(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player) || other.TryGetComponent(out InvulnerableCircle invulnerableCircle))
        {
            _facade.Bounce(other);
        }
    }

    public class Factory : PlaceholderFactory<Asteroid> { }
}