using UnityEngine;

public class UFORotator
{
    private readonly Transform _transform;
    private readonly float _spinningSpeed;
    private readonly int _spinningTurn;

    public UFORotator(Transform transform, float spinningMinSpeed, float spinningMaxSpeed)
    {
        _transform = transform;
        _spinningSpeed = Random.Range(spinningMinSpeed, spinningMaxSpeed + 1);
        _spinningTurn = Random.Range(-1, 2);
    }

    public void Rotate(float deltaTime)
    {
        _transform.Rotate(0, 0, _spinningTurn * deltaTime * _spinningSpeed);
    }
}