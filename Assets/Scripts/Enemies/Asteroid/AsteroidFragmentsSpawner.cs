using UnityEngine;


public class AsteroidFragmentsSpawner
{
    private int _minFragmentAmount;
    private int _maxFragmentAmount;
    private int _fragmentsAmount;
    private Fragment _spawnedFragment;
    private AsteroidConfig _config;
    private FragmentsPool _fragmentsPool;
    private Transform _spawnPoint;
    
    public AsteroidFragmentsSpawner(AsteroidConfig config)
    {
        _config = config;
        _minFragmentAmount = _config.MinFragmentAmount;
        _maxFragmentAmount = _config.MaxFragmentAmount;
    }

    
    public void SpawnFragments()
    {
        _fragmentsAmount = Random.Range(_minFragmentAmount, _maxFragmentAmount + 1);

        for (int i = 0; i < _fragmentsAmount; i++)
        {
            _spawnedFragment = _fragmentsPool.GetFragment();

            if (_spawnedFragment == null) return;
            
            _spawnedFragment.ResetVelocity();
            _spawnedFragment.transform.position = _spawnPoint.position;
            _spawnedFragment.gameObject.SetActive(true);
        }
    }

    public void Init(FragmentsPool pool, Transform spawnPoint)
    {
        _fragmentsPool = pool;
        _spawnPoint =  spawnPoint;
    }
}