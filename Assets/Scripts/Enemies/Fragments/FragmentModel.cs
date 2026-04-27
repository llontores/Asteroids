using UnityEngine;

public class FragmentModel
{
    private const int MaxDegree = 360;
    private readonly FragmentConfig _config;
    private FragmentMover _mover;
    private Transform _transform;

    public int Reward => _config.Reward;

    public FragmentModel()
    {
        _config = JsonConfigLoader.LoadFromResources<FragmentConfig>("Configs/fragment_config");
    }

    public void InitTransform(Transform transform)
    {
        _transform = transform;
        _mover = new FragmentMover(_transform, _config);
    }

    public void OnEnable()
    {
        _transform.Rotate(0, 0, Random.Range(0, MaxDegree + 1));
        _mover.OnEnable();
    }

    public void Update(float deltaTime)
    {
        _mover.Update(deltaTime);
    }

    public void Bounce(Vector2 normal)
    {
        _mover.Bounce(normal);
    }
}