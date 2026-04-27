using UnityEngine;
using Zenject;

public class LaserView : MonoBehaviour
{
    [SerializeField] private ParticleSystem _endPointEffect;

    private LaserShooter _shooter;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(LaserShooter shooter, SignalBus signalBus)
    {
        _shooter = shooter;
        _signalBus = signalBus;
        _signalBus.Subscribe<LaserEndPointUpdatedSignal>(ShowLaserEffect);
        _signalBus.Subscribe<LaserTurnedOffSignal>(HideLaserEffect);
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

    private void OnDisable()
    {
        _signalBus.Unsubscribe<LaserEndPointUpdatedSignal>(ShowLaserEffect);
        _signalBus.Unsubscribe<LaserTurnedOffSignal>(HideLaserEffect);
    }
}