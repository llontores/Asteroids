using System;
using UnityEngine;

public class FragmentsPool : MonoBehaviour
{
    [SerializeField] private Fragment _fragmentPrefab;
    [SerializeField] private int _capacity;
    
    private ObjectPool<Fragment> _asteroidPool;

    private void Start()
    {
        _asteroidPool = new ObjectPool<Fragment>(_capacity, _fragmentPrefab, transform);
    }

    public Fragment GetFragment()
    {
        if (_asteroidPool.TryGetObject(out Fragment fragment))
        {
            fragment.OnDestroy += OnDestroy;
            return fragment;
        }

        return null;
    }

    private void OnDestroy(Fragment fragment)
    {
        fragment.OnDestroy -= OnDestroy;
        _asteroidPool.ReturnObject(fragment);
    }
}