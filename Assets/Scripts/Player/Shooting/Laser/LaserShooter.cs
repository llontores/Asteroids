using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class LaserShooter : IInitializable, IDisposable
{
    private SignalBus _signalBus;
    private float _maxDistance;
    private Transform _shootPoint;
    private LineRenderer _lineRenderer;
    private float _laserDuration;
    private float _cooldown;
    private float _lastShootTime;
    private int _currentAmmoCount;
    private int _maxAmmoCount;
    private float _ammoReloadCooldown;
    private CancellationTokenSource _shootCts;
    private CancellationTokenSource _reloadCts;
    private Camera _camera;
    private float _reloadRemainingTime;
    private float _minX, _maxX, _minY, _maxY;
    private PlayerConfig _playerConfig;
    private PlayerReferences _playerReferences;
    private PlayerFacade _playerFacade;
    private CustomRaycaster _raycaster;
    
    [Inject]
    public void Construct(SignalBus signalBus, PlayerConfig playerConfig, PlayerReferences playerReferences, PlayerFacade playerFacade, CustomRaycaster raycaster)
    {
        _reloadRemainingTime = 0;
        _playerFacade  = playerFacade;
        _playerConfig = playerConfig;
        _signalBus = signalBus;
        _signalBus.Fire(new LaserReloadRemainingTimeChangedSignal{RemainingTime = _reloadRemainingTime});
        _playerReferences = playerReferences;
        _raycaster = raycaster;
    }
    
    public void Initialize()
    {
        _signalBus.Subscribe<LaserShootSignal>(ShootLaser);
        _signalBus.Fire(new LaserTurnedOffSignal());
        _lineRenderer = _playerReferences.LineRenderer;
        _maxDistance = _playerConfig.MaxRayDistance;
        _shootPoint = _playerReferences.ShootPoint;
        _laserDuration = _playerConfig.LaserDuration;
        _cooldown = _playerConfig.LaserCooldown;
        _maxAmmoCount = _playerConfig.MaxLaserAmmoCount;
        _currentAmmoCount = _maxAmmoCount;
        _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal{AmmoCount = _currentAmmoCount});
        _signalBus.Fire(new LaserReloadRemainingTimeChangedSignal{RemainingTime = 0});
        _signalBus.Subscribe<RestartButtonPressedSignal>(ResetLaser);
        
        _ammoReloadCooldown = _playerConfig.LaserReloadCooldown;
        _reloadCts = new CancellationTokenSource();
        _camera = Camera.main;
        
        Vector3 bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        _minX = bottomLeft.x;
        _maxX = topRight.x;
        _minY = bottomLeft.y;
        _maxY = topRight.y;
        
        if (_lineRenderer != null) 
            _lineRenderer.enabled = false;

        _lastShootTime = -_cooldown; 
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<LaserShootSignal>(ShootLaser);
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(ResetLaser);
        _shootCts?.Cancel();
        _shootCts?.Dispose();
        _reloadCts?.Cancel(); 
        _reloadCts?.Dispose();
    }

    private void ShootLaser() => ShootLaserRoutine().Forget();

    private async UniTask ShootLaserRoutine()
    {
        if (Time.time < _lastShootTime + _cooldown || _currentAmmoCount == 0 || _playerFacade.IsInvulnerable)
            return;

        if (_shootCts != null)
        {
            _shootCts.Cancel(); 
            _shootCts.Dispose();
        }

        _currentAmmoCount = Mathf.Clamp(_currentAmmoCount - 1, 0, _maxAmmoCount);
        _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal{AmmoCount = _currentAmmoCount});
        
        if (_currentAmmoCount == 0)
        {
            ReloadLaserAmmo().Forget();
        }
        
        _lastShootTime = Time.time;
        _shootCts = new CancellationTokenSource();
        var token = _shootCts.Token;

        try
        {
            _lineRenderer.enabled = true;
            float elapsedTime = 0;
            while (elapsedTime < _laserDuration)
            {
                UpdateLaser();
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            _signalBus.Fire(new LaserTurnedOffSignal());

        }
        catch (OperationCanceledException) { }
        finally
        {
            if (_lineRenderer != null) _lineRenderer.enabled = false;
            
            if (_shootCts != null) {
                _shootCts.Dispose(); _shootCts = null; 
            }
        }
    }

    private async UniTask ReloadLaserAmmo()
    {
        _reloadRemainingTime = _ammoReloadCooldown;
        while (_reloadRemainingTime > 0)
        {
            _reloadRemainingTime -= Time.deltaTime;
           _signalBus.Fire(new LaserReloadRemainingTimeChangedSignal{RemainingTime = _reloadRemainingTime});
            
            await UniTask.Yield(PlayerLoopTiming.Update, _reloadCts.Token);
        }
        _currentAmmoCount = _maxAmmoCount;
        _signalBus.Fire(new LaserReloadRemainingTimeChangedSignal{RemainingTime = 0});
        _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal{AmmoCount = _currentAmmoCount});
    }

    private void UpdateLaser()
    {
        Vector2 origin = _shootPoint.position;
        Vector2 direction = _shootPoint.up;
        
        Vector2 targetEnd = origin + direction * _maxDistance;
        
        float distanceMultiplier = 1f;  

        if (targetEnd.x < _minX)
            distanceMultiplier = Mathf.Min(distanceMultiplier, (_minX - origin.x) / (targetEnd.x - origin.x));
        if (targetEnd.x > _maxX) 
            distanceMultiplier = Mathf.Min(distanceMultiplier, (_maxX - origin.x) / (targetEnd.x - origin.x));
        if (targetEnd.y < _minY) 
            distanceMultiplier = Mathf.Min(distanceMultiplier, (_minY - origin.y) / (targetEnd.y - origin.y));
        if (targetEnd.y > _maxY) 
            distanceMultiplier = Mathf.Min(distanceMultiplier, (_maxY - origin.y) / (targetEnd.y - origin.y));
        
        float clampedDistance = _maxDistance * distanceMultiplier;
        
        RaycastResult raycastResult = _raycaster.RaycastAll(origin, direction, clampedDistance);

        Vector2 finalPoint = raycastResult.FurthestPoint;

        if (raycastResult.HasHits)
        {
            foreach (var target in raycastResult.HitTargets)
            {
                target.Destroy(DestroyReason.Shootable);
            }
        }
        
        Vector3 renderPoint = new Vector3(finalPoint.x, finalPoint.y, -0.1f);
        _lineRenderer.SetPosition(0, new Vector3(origin.x, origin.y, -0.1f));
        _lineRenderer.SetPosition(1, renderPoint);
        
        _signalBus.Fire(new LaserEndPointUpdatedSignal{LaserEndPoint = renderPoint});
    }

    private void ResetLaser()
    {
        _reloadRemainingTime = 0;
        _currentAmmoCount = _maxAmmoCount;
        _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal{AmmoCount = _currentAmmoCount});
        if (_shootCts != null)
            _shootCts.Cancel();
    }
}