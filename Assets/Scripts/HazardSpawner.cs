using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class HazardSpawner : MonoBehaviour
{
    [SerializeField] private int _minCoolDown;
    [SerializeField] private int _maxCoolDown;
    [SerializeField] private Transform[] _spawnPoints;
    
    [Header("UFO Pool")]
    [SerializeField] private int _ufoCapacity;
    [SerializeField] private UFO _ufoPrefab;
    [SerializeField] private Transform _ufoContainer;
    
    [Header("Asteroid Pool")]
    [SerializeField] private int _asteroidsCapacity;
    [SerializeField] private Asteroid _asteroidPrefab;
    [SerializeField] private Transform _asteroidContainer;

    private ObjectPool<UFO> _ufoPool;
    private ObjectPool<Asteroid> _asteroidPool;

    private bool _isSpawning = true;

    [Inject]
    public void Construct()
    {
        _ufoPool = new ObjectPool<UFO>(_ufoCapacity, _ufoPrefab, _ufoContainer);
        _asteroidPool = new ObjectPool<Asteroid>(_asteroidsCapacity, _asteroidPrefab, _asteroidContainer);
    }

    private void Start()
    {
        SpawnLoop().Forget();
        
    }

    private void OnDisable()
    {
        _isSpawning = false;
    }

    private async UniTaskVoid SpawnLoop()
    {
        var token = this.GetCancellationTokenOnDestroy();

        while (_isSpawning && !token.IsCancellationRequested)
        {
            SpawnOne();
            
            int cooldown = Random.Range(_minCoolDown, _maxCoolDown + 1);
            await UniTask.Delay(cooldown, cancellationToken: token);
        }
    }

    private void SpawnOne()
    {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        bool spawnUfo = Random.Range(0, 2) == 0;

        if (spawnUfo)
        {
            if (_ufoPool.TryGetObject(out var ufo))
            {
                PlaceAndActivate(ufo.transform, spawnPoint);
            }
        }
        else
        {
            if (_asteroidPool.TryGetObject(out var asteroid))
            {
                PlaceAndActivate(asteroid.transform, spawnPoint);
            }
        }
    }

    private void PlaceAndActivate(Transform hazard, Transform spawnPoint)
    {
        hazard.position = spawnPoint.position;
        hazard.rotation = spawnPoint.rotation;
        hazard.gameObject.SetActive(true);
    }
}
