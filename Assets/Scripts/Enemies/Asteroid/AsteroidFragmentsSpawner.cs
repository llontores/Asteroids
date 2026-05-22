using UnityEngine;

public class AsteroidFragmentsSpawner
{
    private int minFragmentAmount;
    private int maxFragmentAmount;
    private AsteroidConfig _config;
    private FragmentsPool _fragmentsPool;
    private Transform _spawnPoint;
    
    public AsteroidFragmentsSpawner(AsteroidConfig config)
    {
        _config = config;
        minFragmentAmount = _config.MinFragmentAmount;
        maxFragmentAmount = _config.MaxFragmentAmount;
    }

    public void SpawnFragments()
    {
        minFragmentAmount = _config.MinFragmentAmount;
        maxFragmentAmount = _config.MaxFragmentAmount;
        int fragmentsAmount = Random.Range(minFragmentAmount, maxFragmentAmount + 1);

        for (int i = 0; i < fragmentsAmount; i++)
        {
            Fragment spawnedFragment = _fragmentsPool.GetFragment();

            if (spawnedFragment == null) return;
            
            spawnedFragment.transform.position = _spawnPoint.position;
            spawnedFragment.gameObject.SetActive(true);
        }
    }

    public void Init(FragmentsPool pool, Transform spawnPoint)
    {
        _fragmentsPool = pool;
        _spawnPoint = spawnPoint;
    }
}