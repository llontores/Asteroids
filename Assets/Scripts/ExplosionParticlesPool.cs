using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Signals;
using UnityEngine;
using Zenject;

public class ExplosionParticlesPool : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private Transform _particlesSystemContainer;
    [SerializeField] private int _particlesContainerCapacity;

    private ObjectPool<ParticleSystem> _particlesPool;
    private CancellationTokenSource _particlesPoolCts;
    private float _effectDuration;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<DestroyableDiedSignal>(SetParticles);
        _particlesPool = new ObjectPool<ParticleSystem>(_particlesContainerCapacity, _explosionParticles,
            _particlesSystemContainer);
        _effectDuration = _explosionParticles.main.duration;
        _particlesPoolCts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<DestroyableDiedSignal>(SetParticles);
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

    private async UniTaskVoid LaunchParticles(ParticleSystem particle)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_effectDuration), cancellationToken: _particlesPoolCts.Token);
        particle.gameObject.SetActive(false);
        _particlesPool.ReturnObject(particle);
    }
}