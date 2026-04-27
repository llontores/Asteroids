using UnityEngine;
using Zenject;

public class GameOverScreen : MonoBehaviour
{
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void RestartGame()
    {
        _signalBus.Fire(new RestartButtonPressedSignal());
    }
}