using UnityEngine;
using Zenject;

public class FragmentsPool
{
    private ObjectPool<Fragment> _pool;
    private SignalBus _signalBus;
    private TargetsRegistry _targetsRegistry;

    public FragmentsPool(int capacity, Fragment.Factory factory, Transform container, SignalBus signalBus, TargetsRegistry targetsRegistry)
    {
        _signalBus = signalBus;
        _targetsRegistry = targetsRegistry;
        _pool = new ObjectPool<Fragment>(capacity, factory, container);
    }

    public Fragment GetFragment()
    {
        if (_pool.TryGetObject(out Fragment fragment))
        {
            fragment.Facade.ResetVelocity();
            fragment.Facade.OnDead += ReturnFragmentToPool;
            _targetsRegistry.Register(fragment.Facade);
            return fragment;
        }
        return null;
    }

    private void ReturnFragmentToPool(FragmentFacade facade)
    {
        facade.OnDead -= ReturnFragmentToPool;
        _targetsRegistry.Unregister(facade);
        _signalBus.Fire(new DestroyableDiedSignal { Entity = facade.GetView() });
        _pool.ReturnObject(facade.GetView());
    }

    public void ResetPool()
    {
        _pool.ResetPool();
    }
}