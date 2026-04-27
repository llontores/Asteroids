using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Fragment : Entity, IDestroyable
{
    public event UnityAction<Fragment> OnDestroy;
    
    private FragmentModel _model;

    [Inject]
    public void Construct(FragmentModel model)
    {
        _model = model;
        _model.InitTransform(transform);
        _reward = _model.Reward;
    }

    private void OnEnable()
    {
        _model?.OnEnable();
    }

    private void Update()
    {
        _model?.Update(Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player) || other.TryGetComponent(out InvulnerableCircle invulnerableCircle))
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            Vector2 normal = ((Vector2)transform.position - contactPoint).normalized;

            _model.Bounce(normal);
        }
    }

    public void Destroy(DestroyReason reason)
    {
        SetDestroyReason(reason);
        OnDestroy?.Invoke(this);
    }
}