using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class FragmentsPool : ObjectPool<Fragment>
{
    private RewardCounter _rewardCounter;
    private UnityAction<int> _rewardableDied;

    [Inject]
    public void Construct(RewardCounter rewardCounter)
    {
        _rewardCounter = rewardCounter;
    }
    
    public FragmentsPool(int capacity, Fragment prefab, Transform container, UnityAction<int> rewardableDied) : base(capacity, prefab, container)
    {
        _rewardableDied = rewardableDied;
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
        _rewardableDied?.Invoke(fragment.Reward);
        ReturnObject(fragment);
    }
}