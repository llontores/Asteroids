using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HazardSpawnerController : IInitializable, IDisposable
{
    private HazarSpawnerConfig _config;
    private SignalBus _signalBus;
    private HazardSpawnerView _view;
    private Transform _target;
    
    private ObjectPool<UFO> _ufoPool;
    private ObjectPool<Asteroid> _asteroidsPool;
    private FragmentsPool _fragmentsPool;
    private CancellationTokenSource _cts;

    [Inject]
    public void Construct(HazardSpawnerView view, SignalBus signalBus, Player player)
    {
        _view = view;
        _signalBus = signalBus;
        _target = player.transform;
        _config = JsonConfigLoader.LoadFromResources<HazarSpawnerConfig>("Configs/hazardSpawner_config");
    }

    public void Initialize()
    {
        _ufoPool = new ObjectPool<UFO>(_config.UFOCapacity, _view.UfoPrefab, _view.UfoContainer);
        _asteroidsPool = new ObjectPool<Asteroid>(_config.AsteroidsCapacity, _view.AsteroidPrefab, _view.AsteroidContainer);
        _fragmentsPool = new FragmentsPool(_config.FragmentsCapacity, _view.FragmentPrefab, _view.FragmentContainer, _signalBus);

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
        Transform spawnPoint = _view.GetRandomSpawnPoint();
        bool spawnUfo = Random.Range(0, 2) == 0;

        if (spawnUfo)
        {
            if (_ufoPool.TryGetObject(out UFO ufo))
            {
                _view.Place(ufo.transform, spawnPoint);
                ufo.InitTarget(_target);
                ufo.OnDestroy += ReturnUFOToPool;
            }
        }
        else
        {
            if (_asteroidsPool.TryGetObject(out Asteroid asteroid))
            {
                _view.Place(asteroid.transform, spawnPoint);
                asteroid.SetDirection(spawnPoint.up);
                asteroid.Init(_fragmentsPool);
                asteroid.OnDead += ReturnAsteroidToPool;
            }
        }
    }

    private void ReturnAsteroidToPool(Asteroid asteroid)
    {
        asteroid.OnDead -= ReturnAsteroidToPool;
        _signalBus.Fire(new DestroyableDiedSignal { Entity = asteroid });
        _asteroidsPool.ReturnObject(asteroid);
    }

    private void ReturnUFOToPool(UFO ufo)
    {
        ufo.OnDestroy -= ReturnUFOToPool;
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