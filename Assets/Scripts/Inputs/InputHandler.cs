using Signals;
using Zenject;

public abstract class InputHandler 
{
    protected SignalBus _signalBus;

    [Inject]
    protected void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    protected void FireAcceleration(bool isPressed)
    {
        _signalBus.Fire(new AccelerationSignal { IsPressed = isPressed });
    }

    protected void FireRotation(int TurnIndex)
    {
        _signalBus.Fire(new TurnSignal{TurnIndex = TurnIndex});
    }

    protected void FireBulletShot()
    {
        _signalBus.Fire(new BulletShootSignal());
    }

    protected void FireLaserShot()
    {
        _signalBus.Fire(new LaserShootSignal());
    }
}