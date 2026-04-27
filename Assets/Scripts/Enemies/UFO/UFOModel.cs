using UnityEngine;

public class UFOModel
{
    private readonly UFOConfig _config;
    private UFOMover _mover;
    private UFOView _view;
    private Transform _transform;

    public int Reward => _config.Reward;

    public UFOModel()
    {
        _config = JsonConfigLoader.LoadFromResources<UFOConfig>("Configs/ufo_config");
    }

    public void InitTransform(Transform transform)
    {
        _transform = transform;
        _mover = new UFOMover(_transform, _config);
        _view = new UFOView(_transform, _config);
    }

    public void SetTarget(Transform target) => _mover.SetTarget(target);

    public void Update(float deltaTime)
    {
        _mover.Update(deltaTime);
        _view.Rotate(deltaTime);
    }

    public void Bounce(Vector2 normal) => _mover.Bounce(normal);
}