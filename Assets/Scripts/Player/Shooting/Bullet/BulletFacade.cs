using UnityEngine;

public class BulletFacade
{
    private BulletMover _mover;
    private BulletConfig _config;
    private Transform _transform;
    private bool _isUsed;
    
    public BulletFacade(BulletConfig config)
    {
        _config = config;
    }

    public void Init(Transform transform)
    {
        _transform = transform;
        _mover = new BulletMover(_config.Thrust, _config.DragForce, _config.MaxSpeed);
    }

    public void OnBulletEnable()
    {
        _isUsed = false;
        _mover.ResetVelocity();
    }

    public void Update(float deltaTime)
    {
        _mover.Update(deltaTime, _transform);
    }

    public bool TryProcessHit(IDestroyable target)
    {
        if (_isUsed) return false;

        _isUsed = true;
        target.Destroy(DestroyReason.Shootable);
        
        return true;
    }
}