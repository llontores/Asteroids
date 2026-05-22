using UnityEngine;
using Zenject;

public class Asteroid : Entity, IDestroyable 
{
    public AsteroidFacade Facade { get; private set; } 

    [Inject]
    public void Construct(AsteroidFacade facade)
    {
        Facade = facade;
        Facade.Bind(this);
        _reward = Facade.Reward;
    }

    private void OnEnable() => Facade?.OnAsteroidEnable();
    private void Update() => Facade?.Update(Time.deltaTime);

    public void Destroy(DestroyReason reason)
    {
        Facade?.Destroy(reason);
    }

    public void SyncReason(DestroyReason reason)
    {
        SetDestroyReason(reason);
    }

    public class Factory : PlaceholderFactory<Asteroid> { }
}