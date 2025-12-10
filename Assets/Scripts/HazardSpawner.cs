using System;
using Cysharp.Threading.Tasks;
using Signals;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

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

    public ObjectPool<UFO> UFOPool { get; private set; }
    public ObjectPool<Asteroid> AsteroidsPool{get; private set;}

    private bool _isSpawning = true;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        UFOPool = new ObjectPool<UFO>(_ufoCapacity, _ufoPrefab, _ufoContainer);
        AsteroidsPool = new ObjectPool<Asteroid>(_asteroidsCapacity, _asteroidPrefab, _asteroidContainer);
        _signalBus = signalBus;
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<AsteroidDeadSignal>(ReturnAsteroidToPool);
    }

    private void OnDisable()
    {
        _isSpawning = false;
        _signalBus.Unsubscribe<AsteroidDeadSignal>(ReturnAsteroidToPool);
    }

    private void Start()
    {
        SpawnLoop().Forget();
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
            if (UFOPool.TryGetObject(out var ufo))
            {
                PlaceAndActivate(ufo.transform, spawnPoint);
            }
        }
        else
        {
            if (AsteroidsPool.TryGetObject(out var asteroid))
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

    public void ReturnAsteroidToPool(AsteroidDeadSignal args)
    {
        AsteroidsPool.ReturnObject(args.Asteroid);
    }

    public void ReturnUFOToPool(UFO returnedUFO)
    {
        UFOPool.ReturnObject(returnedUFO);
    }
}
