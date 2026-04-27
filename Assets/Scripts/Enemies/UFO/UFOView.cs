using UnityEngine;

public class UFOView
{
    private readonly Transform _transform;
    private readonly float _spinningSpeed;
    private readonly int _spinningTurn;

    public UFOView(Transform transform, UFOConfig config)
    {
        _transform = transform;
        _spinningSpeed = Random.Range(config.SpinningMinSpeed, config.SpinningMaxSpeed + 1);
        _spinningTurn = Random.Range(-1, 2); // LeftTurnIndex / RightTurnIndex
    }

    public void Rotate(float deltaTime)
    {
        _transform.Rotate(0, 0, _spinningTurn * deltaTime * _spinningSpeed);
    }
}