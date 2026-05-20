using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Fragment : Entity, ITarget
{
    public Vector2 Position => transform.position;
    public float ColliderRadius { get; private set; }
    public event UnityAction<Fragment> OnDestroy;
    
    private FragmentFacade _facade;
    private SpriteRenderer _renderer;

    [Inject]
    public void Construct(FragmentFacade facade)
    {
        _facade = facade;
        _facade.Init(transform);
        _reward = _facade.Reward;
        _renderer = GetComponent<SpriteRenderer>();
        ColliderRadius = Mathf.Max(_renderer.bounds.extents.x, _renderer.bounds.extents.y);
    }

    private void OnEnable()
    {
        _facade?.OnFragmentEnable();
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

    public void Destroy(DestroyReason reason)
    {
        SetDestroyReason(reason);
        OnDestroy?.Invoke(this);
    }

    public void ResetVelocity()
    {
        _facade.ResetVelocity();
    }

    public class Factory : PlaceholderFactory<Fragment> { }
}