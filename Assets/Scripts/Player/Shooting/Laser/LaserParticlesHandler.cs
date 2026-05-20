using System;
using UnityEngine;
using Zenject;

public class LaserParticlesHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem _endPointEffect;
    
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void ShowLaserEffect(LaserEndPointUpdatedSignal args)
    {
        _endPointEffect.gameObject.SetActive(true);
        _endPointEffect.transform.position = args.LaserEndPoint;
    }

    private void HideLaserEffect()
    {
        _endPointEffect.gameObject.SetActive(false);
    }

    private void Start()
    {
        _signalBus.Subscribe<LaserEndPointUpdatedSignal>(ShowLaserEffect);
        _signalBus.Subscribe<LaserTurnedOffSignal>(HideLaserEffect);
        
        HideLaserEffect();
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<LaserEndPointUpdatedSignal>(ShowLaserEffect);
        _signalBus.Unsubscribe<LaserTurnedOffSignal>(HideLaserEffect);
    }
}