using System;
using Zenject;

public class MobileInput : InputHandler, IDisposable
{
    private MobileButtonsHandler _handler;
    
    [Inject]
    public void Construct(MobileButtonsHandler handler)
    {
        _handler = handler;
        _handler.gameObject.SetActive(true);
        _handler.Accelerate.ButtonPressed += AccelerationButtonPressed;
        _handler.Accelerate.ButtonReleased += AccelerationButtonReleased;
        _handler.TurnLeft.ButtonPressed += TurnLeftButtonPressed;
        _handler.TurnRight.ButtonPressed += TurnRightButtonPressed;
        _handler.ShootBullet.ButtonPressed += ShootBulletButtonPressed;
        _handler.ShootLaser.ButtonPressed += ShootLaserButtonPressed;
    }

    private void AccelerationButtonPressed()
    {
        FireAcceleration(true);
    }

    private void AccelerationButtonReleased()
    {
        FireAcceleration(false);
    }

    private void TurnLeftButtonPressed()
    {
        FireRotation(1);
    }

    private void TurnRightButtonPressed()
    {
        FireRotation(-1);
    }

    private void ShootBulletButtonPressed()
    {
        FireBulletShot();
    }

    private void ShootLaserButtonPressed()
    {
        FireLaserShot();
    }

    public void Dispose()
    {
        _handler.Accelerate.ButtonPressed -= AccelerationButtonPressed;
        _handler.Accelerate.ButtonReleased -= AccelerationButtonReleased;
        _handler.TurnLeft.ButtonPressed -= TurnLeftButtonPressed;
        _handler.TurnRight.ButtonPressed -= TurnRightButtonPressed;
        _handler.ShootBullet.ButtonPressed -= ShootBulletButtonPressed;
        _handler.ShootLaser.ButtonPressed -= ShootLaserButtonPressed;
    }
}