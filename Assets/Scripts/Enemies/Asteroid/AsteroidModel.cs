using UnityEngine;

public class AsteroidModel
{
    public int Reward => _config.Reward;

    private AsteroidMover _mover;
    private AsteroidView _view;
    private FragmentsPool _fragmentsPool;
    private AsteroidConfig _config;
    private Transform _transform;
    private int _minFragmentAmount;
    private int _maxFragmentAmount;
    private int _fragmentsAmount;
    private Fragment _spawnedFragment;

    public AsteroidModel()
    {
        _config = JsonConfigLoader.LoadFromResources<AsteroidConfig>("Configs/asteroid_config");
        _minFragmentAmount = _config.MinFragmentAmount;
        _maxFragmentAmount = _config.MaxFragmentAmount;

        _mover = new AsteroidMover(_config.Thrust, _config.Drag, _config.MaxSpeed, _config.BounceForce);
    }

    public void Init(Transform transform)
    {
        _transform = transform;
        _view = new AsteroidView(_transform, _config.SpinningMinSpeed, _config.SpinningMaxSpeed);
    }

    public void InitPool(FragmentsPool pool)
    {
        _fragmentsPool = pool;
    }

    public void OnEnable()
    {
        _mover.ResetVelocity();
    }

    public void Update(float deltaTime)
    {
        _mover.UpdateForces(deltaTime);
        _view.UpdateTransform(_mover.Velocity, deltaTime);
    }

    public void SetDirection(Vector3 direction)
    {
        _mover.SetDirection(direction);
    }

    public void Bounce(Vector2 normal)
    {
        _mover.Bounce(normal);
    }

    public void Destroy(DestroyReason reason)
    {
        if (reason != DestroyReason.World)
        {
            SpawnFragments();
        }
    }

    private void SpawnFragments()
    {
        _fragmentsAmount = Random.Range(_minFragmentAmount, _maxFragmentAmount + 1);

        for (int i = 0; i < _fragmentsAmount; i++)
        {
            _spawnedFragment = _fragmentsPool.GetFragment();
            _spawnedFragment.transform.position = _transform.position;
            _spawnedFragment.gameObject.SetActive(true);
        }
    }
}