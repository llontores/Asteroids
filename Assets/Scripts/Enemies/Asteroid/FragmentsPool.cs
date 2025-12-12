using UnityEngine;

public class FragmentsPool : MonoBehaviour
{
    [SerializeField] private Fragment _fragmentPrefab;
    [SerializeField] private int _capacity;
    
    private ObjectPool<Fragment> _fragmentsPool;

    private void Start()
    {
        _fragmentsPool = new ObjectPool<Fragment>(_capacity, _fragmentPrefab, transform);
    }

    public Fragment GetFragment()
    {
        if (_fragmentsPool.TryGetObject(out Fragment fragment))
        {
            fragment.OnDestroy += ReturnFragmentToPool;
            return fragment;
        }

        return null;
    }

    private void ReturnFragmentToPool(Fragment fragment)
    {
        fragment.OnDestroy -= ReturnFragmentToPool;
        _fragmentsPool.ReturnObject(fragment);
    }
}