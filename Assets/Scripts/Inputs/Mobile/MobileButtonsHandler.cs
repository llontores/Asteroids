using UnityEngine;

public class MobileButtonsHandler : MonoBehaviour
{
    [SerializeField] private HoldingButton _accelerate;
    [SerializeField] private HoldingButton _turnLeft;
    [SerializeField] private HoldingButton _turnRight;
    [SerializeField] private ClickButton _shootBullet;
    [SerializeField] private ClickButton _shootLaser;

    public HoldingButton Accelerate => _accelerate;
    public HoldingButton TurnLeft => _turnLeft;
    public HoldingButton TurnRight => _turnRight;
    public ClickButton ShootBullet => _shootBullet;
    public ClickButton ShootLaser => _shootLaser;
}