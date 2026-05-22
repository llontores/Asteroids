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
    private ObjectPool<UFO> _ufoPool;
    private ObjectPool<Asteroid> _asteroidsPool;
    private FragmentsPool _fragmentsPool;
    private CancellationTokenSource _cts;
    private TargetsRegistry _targetsRegistry;
    
    [Inject]
    public void Construct(HazardSpawnerReferences references, SignalBus signalBus, Player player,
        HazardSpawnerConfig spawnerConfig, TargetsRegistry targetsRegistry, 
        UFO.Factory ufoFactory, Asteroid.Factory asteroidFactory, FragmentsPool fragmentsPool)
    {
        _references = references;
        _signalBus = signalBus;
        _target = player.transform;
        _config = spawnerConfig;
        _targetsRegistry = targetsRegistry;
        _asteroidFactory = asteroidFactory;
        _ufoFactory = ufoFactory;
        _fragmentsPool = fragmentsPool;
    }

    public void Initialize()
    {
        _ufoPool = new ObjectPool<UFO>(_config.UFOCapacity, _ufoFactory, _references.UfoContainer);
        _asteroidsPool = new ObjectPool<Asteroid>(_config.AsteroidsCapacity, _asteroidFactory, _references.AsteroidContainer);
        _signalBus.Subscribe<RestartButtonPressedSignal>(ResetSpawner);
        _signalBus.Subscribe<PlayerDeadSignal>(StopSpawning);
        _cts = new CancellationTokenSource();
        SpawnLoop(_cts.Token).Forget();
    }

    private async UniTask SpawnLoop(CancellationToken token)
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
                ufo.Facade.ResetVelocity();
                ufo.Facade.SetTarget(_target);
                ufo.Facade.OnDead += ReturnUFOToPool;
                _targetsRegistry.Register(ufo.Facade);
                _references.Place(ufo.transform, spawnPoint);
            }
        }
        else
        {
            if (_asteroidsPool.TryGetObject(out Asteroid asteroid))
            {
                asteroid.Facade.SetDirection(spawnPoint.up);
                asteroid.Facade.OnDead += ReturnAsteroidToPool;
                _targetsRegistry.Register(asteroid.Facade);
                _references.Place(asteroid.transform, spawnPoint);
            }
        }
    }

    private void ReturnAsteroidToPool(AsteroidFacade facade)
    {
        facade.OnDead -= ReturnAsteroidToPool;
        _targetsRegistry.Unregister(facade);
        _signalBus.Fire(new DestroyableDiedSignal { Entity = facade.GetView()});
        _asteroidsPool.ReturnObject(facade.GetView());
    }

    private void ReturnUFOToPool(UFOFacade facade)
    {
        facade.OnDead -= ReturnUFOToPool;
        _targetsRegistry.Unregister(facade);
        _signalBus.Fire(new DestroyableDiedSignal { Entity = facade.GetView() });
        _ufoPool.ReturnObject(facade.GetView());
    }

    private void ResetSpawner()
    {
        _ufoPool.ResetPool();
        _asteroidsPool.ResetPool();
        _fragmentsPool.ResetPool();
    }

    private void StopSpawning()
    {
        _cts?.Cancel();
    }
    
    public void Dispose()
    {
        _cts?.Cancel();
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(ResetSpawner);
        _signalBus.Unsubscribe<PlayerDeadSignal>(StopSpawning);
    }
}