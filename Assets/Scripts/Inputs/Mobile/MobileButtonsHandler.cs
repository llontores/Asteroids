using UnityEngine;

public class MobileButtonsHandler : MonoBehaviour
{
    [SerializeField] private ActionButton _accelerate;
    [SerializeField] private ActionButton _turnLeft;
    [SerializeField] private ActionButton _turnRight;
    [SerializeField] private ActionButton _shootBullet;
    [SerializeField] private ActionButton _shootLaser;

    public ActionButton Accelerate => _accelerate;
    public ActionButton TurnLeft => _turnLeft;
    public ActionButton TurnRight => _turnRight;
    public ActionButton ShootBullet => _shootBullet;
    public ActionButton ShootLaser => _shootLaser;
}