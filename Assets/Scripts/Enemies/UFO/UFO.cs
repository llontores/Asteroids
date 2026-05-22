using UnityEngine;
using Zenject;

public class UFO : Entity, IDestroyable
{
    public UFOFacade Facade { get; private set; }

    [Inject]
    public void Construct(UFOFacade facade)
    {
        Facade = facade;
        Facade.Bind(this);
        _reward = Facade.Reward;
    }

    private void Update()
    {
        Facade?.Update(Time.deltaTime);
    }

    public void Destroy(DestroyReason reason)
    {
        Facade?.Destroy(reason);
    }

    public void SyncReason(DestroyReason reason)
    {
        SetDestroyReason(reason);
    }
    
    public class Factory : PlaceholderFactory<UFO> { }
}