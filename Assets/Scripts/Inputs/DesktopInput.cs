using Signals;
using Zenject;
using UnityEngine;


public class DesktopInput : InputHandler, ITickable
{
    [Inject]
    public void Construct(MobileButtonsHandler buttonsHandler)
    {
        buttonsHandler.gameObject.SetActive(false);
    }
    
    public void Tick()
    {
        if (Input.GetKeyUp(KeyCode.W))
            FireAcceleration(false);

        if (Input.GetKey(KeyCode.W))
            FireAcceleration(true);

        if (Input.GetKey(KeyCode.A))
            FireRotation(1);

        if (Input.GetKey(KeyCode.D))
            FireRotation(-1);

        if (Input.GetMouseButtonDown(0))
            FireBulletShot();

        if (Input.GetMouseButtonDown(1))
            FireLaserShot();
    }
}