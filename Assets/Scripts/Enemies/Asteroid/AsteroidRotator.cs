using UnityEngine;

public class AsteroidRotator
{
    private const int LeftTurnIndex = 1;
    private const int RightTurnIndex = -1;

    private Transform _transform;
    private int _spinningTurn;
    private float _spinningSpeed;

    public AsteroidRotator(Transform transform, float spinningMinSpeed, float spinningMaxSpeed)
    {
        _transform = transform;
        _spinningSpeed = Random.Range(spinningMinSpeed, spinningMaxSpeed + 1);
        _spinningTurn = Random.Range(RightTurnIndex, LeftTurnIndex + 1);
    }

    public void Spin(float deltaTime)
    {
        _transform.Rotate(0, 0, _spinningTurn * deltaTime * _spinningSpeed);
    }
}