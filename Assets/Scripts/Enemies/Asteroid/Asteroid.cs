using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Asteroid : Entity, IDestroyable
{
    public event UnityAction<Asteroid> OnDead;

    private AsteroidModel _model;

    [Inject]
    public void Construct(AsteroidModel model)
    {
        _model = model;
        _model.Init(transform);
        _reward = _model.Reward;
    }

    private void OnEnable()
    {
        _model?.OnEnable();
    }

    public void Update()
    {
        _model?.Update(Time.deltaTime);
    }

    public void Init(FragmentsPool pool)
    {
        _model.InitPool(pool);
    }

    public void SetDirection(Vector3 direction)
    {
        _model.SetDirection(direction);
    }

    public void Destroy(DestroyReason reason)
    {
        SetDestroyReason(reason);
        _model.Destroy(reason);
        OnDead?.Invoke(this);
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
}