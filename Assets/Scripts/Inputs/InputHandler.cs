using Zenject;

public abstract class InputHandler 
{
    protected SignalBus _signalBus;

    [Inject]
    protected void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    protected void FireAcceleration(float power)
    {
        _signalBus.Fire(new AccelerationSignal { Power = power });
    }

    protected void FireRotation(int turnIndex)
    {
        _signalBus.Fire(new TurnSignal{TurnIndex = turnIndex});
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