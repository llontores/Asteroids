using UnityEngine;
using Zenject;

public class FragmentsPool : ObjectPool<Fragment>
{
    private SignalBus _signalBus;
    private TargetsRegistry _targetsRegistry;
    
    public FragmentsPool(int capacity, IFactory<Fragment> prefab, Transform container, 
        SignalBus signalBus, TargetsRegistry targetsRegistry) : base(capacity, prefab, container)
    {
        _signalBus = signalBus;
        _targetsRegistry = targetsRegistry;   
    }

    public Fragment GetFragment()
    {
        if (TryGetObject(out Fragment fragment))
        {
            fragment.OnDestroy += ReturnFragmentToPool;
            _targetsRegistry.Register(fragment);
            return fragment;
        }

        return null;
    }

    private void ReturnFragmentToPool(Fragment fragment)
    {
        fragment.OnDestroy -= ReturnFragmentToPool;
        _targetsRegistry.Unregister(fragment);
        _signalBus.Fire(new DestroyableDiedSignal{Entity = fragment});
        ReturnObject(fragment);
    }
}