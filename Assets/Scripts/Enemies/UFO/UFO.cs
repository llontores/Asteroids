using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class UFO : Entity, ITarget
{
    public event UnityAction<UFO> Destroyed;
    public float ColliderRadius { get; private set; }
    public Vector2 Position => transform.position;

    private SpriteRenderer _renderer;
    private UFOFacade _facade;

    [Inject]
    public void Construct(UFOFacade facade)
    {
        _facade = facade;
        _facade.InitTransform(transform);
        _reward = _facade.Reward;
        _renderer = GetComponent<SpriteRenderer>();
        ColliderRadius = Mathf.Max(_renderer.bounds.extents.x, _renderer.bounds.extents.y);
    }

    private void Update()
    {
        _facade?.Update(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player) || other.TryGetComponent(out InvulnerableCircle invulnerableCircle))
        {
            _facade.Bounce(other);
        }
    }

    public void ResetVelocity()
    {
        _facade.ResetVelocity();
    }
    
    public void InitTarget(Transform target)
    {
        _facade.SetTarget(target);
    }

    public void Destroy(DestroyReason reason)
    {
        SetDestroyReason(reason);
        Destroyed?.Invoke(this);
    }
    
    public class Factory : PlaceholderFactory<UFO> { }
}