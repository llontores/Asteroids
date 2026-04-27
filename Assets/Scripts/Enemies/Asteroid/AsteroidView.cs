using UnityEngine;

public class AsteroidView
{
    private const int LeftTurnIndex = 1;
    private const int RightTurnIndex = -1;

    private Transform _transform;
    private int _spinningTurn;
    private float _spinningSpeed;

    public AsteroidView(Transform transform, float spinningMinSpeed, float spinningMaxSpeed)
    {
        _transform = transform;
        _spinningSpeed = Random.Range(spinningMinSpeed, spinningMaxSpeed + 1);
        _spinningTurn = Random.Range(RightTurnIndex, LeftTurnIndex + 1);
    }

    public void UpdateTransform(Vector2 velocity, float deltaTime)
    {
        _transform.position += (Vector3)(velocity * deltaTime);
        _transform.Rotate(0, 0, _spinningTurn * deltaTime * _spinningSpeed);
    }
}