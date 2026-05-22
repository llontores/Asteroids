using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ExplosionParticlesPool : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private Transform _particlesSystemContainer;
    
    private int _particlesContainerCapacity;
    private ObjectPool<ParticleSystem> _particlesPool;
    private CancellationTokenSource _sessionCts;
    private float _effectDuration;
    private SignalBus _signalBus;
    private ExplosionParticlesPoolConfig _config;

    [Inject]
    public void Construct(SignalBus signalBus, ExplosionParticlesPoolConfig config)
    {
        _signalBus = signalBus;
        _config = config;
        
        _particlesContainerCapacity = _config.Capacity;
        _particlesPool = new ObjectPool<ParticleSystem>(_particlesContainerCapacity, _explosionParticles, _particlesSystemContainer);
        _effectDuration = _explosionParticles.main.duration;

        _sessionCts = new CancellationTokenSource();
    }

    private void ResetPoolForNewGame()
    {
        _sessionCts.Cancel();
        _sessionCts.Dispose();
        _sessionCts = new CancellationTokenSource();

        foreach (Transform child in _particlesSystemContainer)
        {
            if (child.gameObject.activeSelf)
            {
                var ps = child.GetComponent<ParticleSystem>();
                ps.Stop();
                child.gameObject.SetActive(false);
                _particlesPool.ReturnObject(ps);
            }
        }
    }

    private void Start()
    {
        _signalBus.Subscribe<DestroyableDiedSignal>(SetParticles);
        _signalBus.Subscribe<PlayerDeadSignal>(ResetPoolForNewGame);
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<DestroyableDiedSignal>(SetParticles);
        _signalBus.Unsubscribe<PlayerDeadSignal>(ResetPoolForNewGame);
        
        if (_sessionCts != null)
        {
            _sessionCts.Cancel();
            _sessionCts.Dispose();
        }
    }

    private void SetParticles(DestroyableDiedSignal args)
    {
        if (_particlesPool.TryGetObject(out ParticleSystem particle))
        {
            particle.gameObject.transform.position = args.Entity.gameObject.transform.position;
            particle.gameObject.SetActive(true);
            particle.Play();
            LaunchParticles(particle).Forget();
        }
    }

    private async UniTask LaunchParticles(ParticleSystem particle)
    {
        bool isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(_effectDuration), 
            cancellationToken: _sessionCts.Token).SuppressCancellationThrow();

        if (isCancelled || particle == null) return;

        particle.gameObject.SetActive(false);
        _particlesPool.ReturnObject(particle);
    }
}