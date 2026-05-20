using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HazardSpawnerController : IInitializable, IDisposable
{
    private HazardSpawnerConfig _config;
    private SignalBus _signalBus;
    private HazardSpawnerReferences _references;
    private Transform _target;
    private Asteroid.Factory _asteroidFactory;
    private UFO.Factory _ufoFactory;
    private Fragment.Factory _fragmentFactory;
    private ObjectPool<UFO> _ufoPool;
    private ObjectPool<Asteroid> _asteroidsPool;
    private FragmentsPool _fragmentsPool;
    private CancellationTokenSource _cts;
    private TargetsRegistry _targetsRegistry;
    
    [Inject]
    public void Construct(HazardSpawnerReferences references, SignalBus signalBus, Player player,
        HazardSpawnerConfig spawnerConfig, TargetsRegistry targetsRegistry, 
        UFO.Factory ufoFactory, Asteroid.Factory asteroidFactory, Fragment.Factory fragmentsFactory)
    {
        _references = references;
        _signalBus = signalBus;
        _target = player.transform;
        _config = spawnerConfig;
        _targetsRegistry = targetsRegistry;
        _asteroidFactory = asteroidFactory;
        _ufoFactory = ufoFactory;
        _fragmentFactory = fragmentsFactory;
    }

    public void Initialize()
    {
        _ufoPool = new ObjectPool<UFO>(_config.UFOCapacity, _ufoFactory, _references.UfoContainer);
        _asteroidsPool = new ObjectPool<Asteroid>(_config.AsteroidsCapacity, _asteroidFactory, _references.AsteroidContainer);
        _fragmentsPool = new FragmentsPool(_config.FragmentsCapacity, _fragmentFactory, _references.FragmentContainer, _signalBus, _targetsRegistry);
        _signalBus.Subscribe<RestartButtonPressedSignal>(ResetSpawner);
        
        _cts = new CancellationTokenSource();
        SpawnLoop(_cts.Token).Forget();
    }

    private async UniTaskVoid SpawnLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            SpawnOne();
            int cooldown = Random.Range(_config.MinCoolDown, _config.MaxCoolDown + 1);
            await UniTask.Delay(cooldown, cancellationToken: token);
        }
    }

    private void SpawnOne()
    {
        Transform spawnPoint = _references.GetRandomSpawnPoint();
        bool spawnUfo = Random.Range(0, 2) == 0;

        if (spawnUfo)
        {
            if (_ufoPool.TryGetObject(out UFO ufo))
            {
                ufo.ResetVelocity();
                ufo.InitTarget(_target);
                ufo.Destroyed += ReturnUFOToPool;
                _targetsRegistry.Register(ufo);
                _references.Place(ufo.transform, spawnPoint);
            }
        }
        else
        {
            if (_asteroidsPool.TryGetObject(out Asteroid asteroid))
            {
                asteroid.SetDirection(spawnPoint.up);
                asteroid.Init(_fragmentsPool);
                asteroid.OnDead += ReturnAsteroidToPool;
                _targetsRegistry.Register(asteroid);
                _references.Place(asteroid.transform, spawnPoint);
            }
        }
    }

    private void ReturnAsteroidToPool(Asteroid asteroid)
    {
        asteroid.OnDead -= ReturnAsteroidToPool;
        _targetsRegistry.Unregister(asteroid);
        _signalBus.Fire(new DestroyableDiedSignal { Entity = asteroid });
        _asteroidsPool.ReturnObject(asteroid);
    }

    private void ReturnUFOToPool(UFO ufo)
    {
        ufo.Destroyed -= ReturnUFOToPool;
        _targetsRegistry.Unregister(ufo);
        _signalBus.Fire(new DestroyableDiedSignal { Entity = ufo });
        _ufoPool.ReturnObject(ufo);
    }

    private void ResetSpawner()
    {
        _ufoPool.ResetPool();
        _asteroidsPool.ResetPool();
        _fragmentsPool.ResetPool();
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(ResetSpawner);
    }
}