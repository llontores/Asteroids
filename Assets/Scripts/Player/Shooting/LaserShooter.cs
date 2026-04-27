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
    private LayerMask _layerMaskIgnore;
    private float _cooldown;
    private float _lastShootTime;
    private int _currentAmmoCount;
    private int _maxAmmoCount;
    private float _ammoReloadCooldown;
    private int _shootedAmount;
    private Player _player;
    private CancellationTokenSource _shootCts;
    private CancellationTokenSource _reloadCts;
    private readonly RaycastHit2D[] _hitsBuffer = new RaycastHit2D[10];
    private Camera _camera;
    private float _reloadRemainingTime;
    private float _minX, _maxX, _minY, _maxY;

    
    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _reloadRemainingTime = 0;
        _player = player;
        _signalBus = signalBus;
        _signalBus.Fire(new LaserReloadRemainingTimeChangedSIgnal{RemainingTime = _reloadRemainingTime});
    }
    
    public void Initialize()
    {
        _signalBus.Subscribe<LaserShootSignal>(ShootLaser);
        _signalBus.Fire(new LaserTurnedOffSignal());
        _lineRenderer = _player.LineRenderer;
        _maxDistance = _player.Config.MaxRayDistance;
        _shootPoint = _player.ShootPoint;
        _laserDuration = _player.Config.LaserDuration;
        _layerMaskIgnore = _player.LayerMaskIgnore;
        _cooldown = _player.Config.LaserCooldown;
        _maxAmmoCount = _player.Config.MaxLaserAmmoCount;
        _currentAmmoCount = _maxAmmoCount;
        _signalBus.Fire(new LaserRemainingAmmoCountUpdatedSignal{AmmoCount = _currentAmmoCount});
        _signalBus.Fire(new LaserReloadRemainingTimeChangedSIgnal{RemainingTime = 0});
        _signalBus.Subscribe<RestartButtonPressedSignal>(ResetLaser);
        
        _ammoReloadCooldown = _player.Config.LaserReloadCooldown;
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

    private async UniTaskVoid ShootLaserRoutine()
    {
        if (Time.time < _lastShootTime + _cooldown || _currentAmmoCount == 0 || _player.IsInvulnerable)
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
            if (_shootCts != null) { _shootCts.Dispose(); _shootCts = null; }
        }
    }

    private async UniTaskVoid ReloadLaserAmmo()
    {
        _reloadRemainingTime = _ammoReloadCooldown;
        while (_reloadRemainingTime > 0)
        {
            _reloadRemainingTime -= Time.deltaTime;
           _signalBus.Fire(new LaserReloadRemainingTimeChangedSIgnal{RemainingTime = _reloadRemainingTime});
            
            await UniTask.Yield(PlayerLoopTiming.Update, _reloadCts.Token);
        }
        _currentAmmoCount = _maxAmmoCount;
        _signalBus.Fire(new LaserReloadRemainingTimeChangedSIgnal{RemainingTime = 0});
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
        
        int hitCount = Physics2D.RaycastNonAlloc(origin, direction, _hitsBuffer, clampedDistance, ~_layerMaskIgnore);

        Vector2 finalPoint;

        if (hitCount > 0)
        {
            float maxDist = -1f;
            Vector2 furthest = origin;
            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hitsBuffer[i];
                if (hit.distance > maxDist)
                {
                    maxDist = hit.distance;
                    furthest = hit.point;
                }
                hit.collider.GetComponent<IDestroyable>()?.Destroy(DestroyReason.Shootable);
            }
            finalPoint = furthest;
        }
        else
        {
            finalPoint = origin + direction * clampedDistance;
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
    }
}