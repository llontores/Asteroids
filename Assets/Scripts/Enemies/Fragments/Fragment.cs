using UnityEngine;
using Zenject;

public class Fragment : Entity, IDestroyable
{
    public FragmentFacade Facade { get; private set; }

    [Inject]
    public void Construct(FragmentFacade facade)
    {
        Facade = facade;
        Facade.Bind(this);
        _reward = Facade.Reward;
    }

    private void OnEnable()
    {
        Facade?.OnFragmentEnable();
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

    public class Factory : PlaceholderFactory<Fragment> { }
}