using Signals;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class FragmentsPool : ObjectPool<Fragment>
{
    private SignalBus _signalBus;
    
    public FragmentsPool(int capacity, Fragment prefab, Transform container, SignalBus signalBus) : base(capacity, prefab, container)
    {
        _signalBus = signalBus;
    }

    public Fragment GetFragment()
    {
        if (TryGetObject(out Fragment fragment))
        {
            fragment.OnDestroy += ReturnFragmentToPool;
            return fragment;
        }

        return null;
    }

    private void ReturnFragmentToPool(Fragment fragment)
    {
        fragment.OnDestroy -= ReturnFragmentToPool;
        _signalBus.Fire(new DestroyableDiedSignal{Entity = fragment});
        ReturnObject(fragment);
    }
}