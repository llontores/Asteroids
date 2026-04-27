using UnityEngine;
using Zenject;

public class RestartButtonPressedSignal : MonoBehaviour
{
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void PressButton()
    {
        _signalBus.Fire(new RestartButtonPressedSignal());
    }
}