using UnityEngine;

public class MobileButtonsHandler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Joystick _joystick;
    
    [Header("Combat")]
    [SerializeField] private ClickButton _shootBullet;
    [SerializeField] private ClickButton _shootLaser;

    public Joystick Joystick => _joystick;
    public ClickButton ShootBullet => _shootBullet;
    public ClickButton ShootLaser => _shootLaser;
}