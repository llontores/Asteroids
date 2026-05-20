using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Bullet : MonoBehaviour, IDestroyable
{
    public event UnityAction<Bullet> OnBulletDestroyed;

    private BulletFacade _facade;

    [Inject]
    public void Construct(BulletFacade facade)
    {
        _facade = facade;
        _facade.Init(transform);
    }

    private void OnEnable()
    {
        _facade?.OnBulletEnable();
    }

    private void Update()
    {
        _facade?.Update(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDestroyable shootable))
        {
            if (_facade.TryProcessHit(shootable))
            {
                Destroy(DestroyReason.Shootable);
            }
        }
    }

    public void Destroy(DestroyReason reason)
    {
        OnBulletDestroyed?.Invoke(this);
    }
    
    public class Factory : PlaceholderFactory<Bullet> { }
}