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
        _handler.Accelerate.Pressed += AccelerationButtonPressed;
        _handler.Accelerate.Released += AccelerationButtonReleased;
        _handler.TurnLeft.Pressed += TurnLeftButtonPressed;
        _handler.TurnRight.Pressed += TurnRightButtonPressed;
        _handler.ShootBullet.Pressed += ShootBulletButtonPressed;
        _handler.ShootLaser.Pressed += ShootLaserButtonPressed;
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
        _handler.Accelerate.Pressed -= AccelerationButtonPressed;
        _handler.Accelerate.Released -= AccelerationButtonReleased;
        _handler.TurnLeft.Pressed -= TurnLeftButtonPressed;
        _handler.TurnRight.Pressed -= TurnRightButtonPressed;
        _handler.ShootBullet.Pressed -= ShootBulletButtonPressed;
        _handler.ShootLaser.Pressed -= ShootLaserButtonPressed;
    }
}