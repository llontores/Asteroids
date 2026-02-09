using Cysharp.Threading.Tasks;
using Signals;
using UnityEngine;
using UnityEngine.Events;
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
    
    [Header("Fragments Pool")]
    [SerializeField] private int _fragmentsPoolCapacity;
    [SerializeField] private Fragment _fragmentPrefab;
    [SerializeField] private Transform _fragmentContainer;

    public event UnityAction<int, DestroyReason> RewardableDied;
    
    private ObjectPool<UFO> _ufoPool;
    private ObjectPool<Asteroid> _asteroidsPool;
    private FragmentsPool _fragmentsPool;
    private bool _isSpawning = true;
    private SignalBus _signalBus;
    private Transform _target;
    

    [Inject]
    public void Construct(Player player, SignalBus signalBus)
    {
        _signalBus = signalBus;
        _target = player.transform;
        _ufoPool = new ObjectPool<UFO>(_ufoCapacity, _ufoPrefab, _ufoContainer);
        _asteroidsPool = new ObjectPool<Asteroid>(_asteroidsCapacity, _asteroidPrefab, _asteroidContainer);
        _fragmentsPool = new FragmentsPool(_fragmentsPoolCapacity, _fragmentPrefab, _fragmentContainer, signalBus);

        for (int i = 0; i < _asteroidContainer.childCount; i++)
        {
            Asteroid spawned = _asteroidContainer.GetChild(i).GetComponent<Asteroid>();
            spawned.Init(_fragmentsPool);
        }
    }

    private void OnDisable()
    {
        _isSpawning = false;
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
            if (_ufoPool.TryGetObject(out UFO ufo))
            {
                PlaceAndActivate(ufo.transform, spawnPoint);
                ufo.InitTarget(_target);
                ufo.OnDestroy += ReturnUFOToPool;
            }
        }
        else
        {
            if (_asteroidsPool.TryGetObject(out Asteroid asteroid))
            {
                PlaceAndActivate(asteroid.transform, spawnPoint);
                asteroid.SetDirection(spawnPoint.up);
                asteroid.OnDead += ReturnAsteroidToPool;
            }
        }
    }

    private void PlaceAndActivate(Transform hazard, Transform spawnPoint)
    {
        hazard.position = spawnPoint.position;
        hazard.rotation = spawnPoint.rotation;
        hazard.gameObject.SetActive(true);
    }

    public void ReturnAsteroidToPool(Asteroid asteroid)
    {
        asteroid.OnDead -= ReturnAsteroidToPool;
        _signalBus.Fire(new DestroyableDiedSignal{Entity = asteroid});
        _asteroidsPool.ReturnObject(asteroid);
    }

    public void ReturnUFOToPool(UFO returnedUFO)
    {
        returnedUFO.OnDestroy -= ReturnUFOToPool;
        _signalBus.Fire(new DestroyableDiedSignal{Entity = returnedUFO});
        _ufoPool.ReturnObject(returnedUFO);
    }
}
