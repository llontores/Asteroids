using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class UFO : Entity, IDestroyable
{
    public event UnityAction<UFO> OnDestroy;
    private UFOModel _model;

    [Inject]
    public void Construct(UFOModel model)
    {
        _model = model;
        _model.InitTransform(transform);
        _reward = _model.Reward;
    }

    private void Update()
    {
        _model?.Update(Time.deltaTime);
    }

    public void InitTarget(Transform target)
    {
        _model.SetTarget(target);
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