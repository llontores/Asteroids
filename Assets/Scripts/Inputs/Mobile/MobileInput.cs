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
        
        _handler.Joystick.OnAccelerateChanged += AccelerationChanged;
        _handler.Joystick.OnTurnChanged += TurnChanged;

        _handler.ShootBullet.ButtonPressed += ShootBulletButtonPressed;
        _handler.ShootLaser.ButtonPressed += ShootLaserButtonPressed;
    }

    public void Dispose()
    {
        _handler.Joystick.OnAccelerateChanged -= AccelerationChanged;
        _handler.Joystick.OnTurnChanged -= TurnChanged;
        _handler.ShootBullet.ButtonPressed -= ShootBulletButtonPressed;
        _handler.ShootLaser.ButtonPressed -= ShootLaserButtonPressed;
    }
    
    private void AccelerationChanged(float power)
    {
        FireAcceleration(power); 
    }

    private void TurnChanged(int turnDirection)
    {
        FireRotation(turnDirection); 
    }

    private void ShootBulletButtonPressed()
    {
        FireBulletShot();
    }

    private void ShootLaserButtonPressed()
    {
        FireLaserShot();
    }
}