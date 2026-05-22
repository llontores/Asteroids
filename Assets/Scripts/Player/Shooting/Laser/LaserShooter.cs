
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class LaserShooter : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly PlayerConfig _playerConfig;
    private readonly PlayerReferences _playerReferences;
    private readonly PlayerFacade _playerFacade;
    private readonly CustomRaycaster _raycaster;
    private readonly Camera _camera;

    private LineRenderer _lineRenderer;
    private Transform _shootPoint;
    private float _maxDistance;
    private float _laserDuration;
    private float _cooldown;
    private float _lastShootTime;
    
    private int _currentAmmoCount;
    private int _maxAmmoCount;
    private float _ammoReloadCooldown;
    private float _reloadRemainingTime;
    
    private CancellationTokenSource _shootCts;
    private CancellationTokenSource _reloadCts;
    
    private float _minX, _maxX, _minY, _maxY;

    public LaserShooter(SignalBus signalBus, PlayerConfig playerConfig, PlayerReferences playerReferences, 
        PlayerFacade playerFacade, CustomRaycaster raycaster, Camera camera)
    {
        _signalBus = signalBus;
        _playerConfig = playerConfig;
        _playerReferences = playerReferences;
        _playerFacade = playerFacade;
        _raycaster = raycaster;
        _camera = camera;
    }

    public void Initialize()
    {
        _lineRenderer = _playerReferences.LineRenderer;
        _shootPoint = _playerReferences.ShootPoint;
        _maxDistance = _playerConfig.MaxRayDistance;
        _laserDuration = _playerConfig.LaserDuration;
        _cooldown = _playerConfig.LaserCooldown;
        _maxAmmoCount = _playerConfig.MaxLaserAmmoCount;
        _ammoReloadCooldown = _playerConfig.LaserReloadCooldown;
        
        _currentAmmoCount = _maxAmmoCount;
        _reloadRemainingTime = 0;
        _lastShootTime = -_cooldown;
        _shootCts = new CancellationTokenSource();
        _reloadCts = new CancellationTokenSource();

        Vector3 bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        _minX = bottomLeft.x;
        _maxX = topRight.x;
        _minY = bottomLeft.y;
        _maxY = topRight.y;

        if (_lineRenderer != null) _lineRenderer.enabled = false;

        _signalBus.Subscribe<LaserShootSignal>(ShootLaser);
        _signalBus.Subscribe<RestartButtonPressedSignal>(ResetLaser);
        _signalBus.Subscribe<PlayerDeadSignal>(StopRoutines);
        
        _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal { AmmoCount = _currentAmmoCount });
        _signalBus.Fire(new LaserReloadRemainingTimeChangedSignal { RemainingTime = 0 });
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<LaserShootSignal>(ShootLaser);
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(ResetLaser);
        _signalBus.Unsubscribe<PlayerDeadSignal>(StopRoutines);
        
        _shootCts.Cancel();
        _shootCts.Dispose();
        _reloadCts.Cancel();
        _reloadCts.Dispose();
    }

    private void ShootLaser() => ShootLaserRoutine().Forget();

    private async UniTask ShootLaserRoutine()
    {
        if (Time.time < _lastShootTime + _cooldown || _currentAmmoCount <= 0 || _playerFacade.IsInvulnerable)
            return;

        _shootCts.Cancel();
        _shootCts = new CancellationTokenSource();

        _currentAmmoCount--;
        _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal { AmmoCount = _currentAmmoCount });

        if (_currentAmmoCount == 0) ReloadLaserAmmo().Forget();

        _lastShootTime = Time.time;

        try
        {
            _lineRenderer.enabled = true;
            float elapsedTime = 0;
            while (elapsedTime < _laserDuration)
            {
                UpdateLaser();
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, _shootCts.Token);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            if (_lineRenderer != null) _lineRenderer.enabled = false;
            _signalBus.Fire(new LaserTurnedOffSignal());
        }
    }

    private async UniTask ReloadLaserAmmo()
    {
        _reloadRemainingTime = _ammoReloadCooldown;
        try
        {
            while (_reloadRemainingTime > 0)
            {
                _reloadRemainingTime -= Time.deltaTime;
                _signalBus.Fire(new LaserReloadRemainingTimeChangedSignal { RemainingTime = _reloadRemainingTime });
                await UniTask.Yield(PlayerLoopTiming.Update, _reloadCts.Token);
            }
            _currentAmmoCount = _maxAmmoCount;
            _signalBus.Fire(new LaserReloadRemainingTimeChangedSignal { RemainingTime = 0 });
            _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal { AmmoCount = _currentAmmoCount });
        }
        catch (OperationCanceledException) { }
    }

    private void UpdateLaser()
    {
        Vector2 origin = _shootPoint.position;
        Vector2 direction = _shootPoint.up;
        Vector2 targetEnd = origin + direction * _maxDistance;
        
        float distMult = 1f;
        if (targetEnd.x < _minX) distMult = Mathf.Min(distMult, (_minX - origin.x) / (targetEnd.x - origin.x));
        if (targetEnd.x > _maxX) distMult = Mathf.Min(distMult, (_maxX - origin.x) / (targetEnd.x - origin.x));
        if (targetEnd.y < _minY) distMult = Mathf.Min(distMult, (_minY - origin.y) / (targetEnd.y - origin.y));
        if (targetEnd.y > _maxY) distMult = Mathf.Min(distMult, (_maxY - origin.y) / (targetEnd.y - origin.y));
        
        RaycastResult result = _raycaster.RaycastAll(origin, direction, _maxDistance * distMult);

        if (result.HasHits)
        {
            foreach (var target in result.HitTargets) target.Destroy(DestroyReason.Laser);
        }

        Vector3 renderPoint = new Vector3(result.FurthestPoint.x, result.FurthestPoint.y, -0.1f);
        _lineRenderer.SetPosition(0, new Vector3(origin.x, origin.y, -0.1f));
        _lineRenderer.SetPosition(1, renderPoint);
        _signalBus.Fire(new LaserEndPointUpdatedSignal { LaserEndPoint = renderPoint });
    }

    private void StopRoutines()
    {
        _shootCts.Cancel();
        _shootCts = new CancellationTokenSource();
        _reloadCts.Cancel();
        _reloadCts = new CancellationTokenSource();
    }

    private void ResetLaser()
    {
        StopRoutines();
        _reloadRemainingTime = 0;
        _currentAmmoCount = _maxAmmoCount;
        _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal { AmmoCount = _currentAmmoCount });
        _signalBus.Fire(new LaserReloadRemainingTimeChangedSignal { RemainingTime = 0 });
    }
}
