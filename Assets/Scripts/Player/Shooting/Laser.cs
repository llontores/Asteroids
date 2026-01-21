using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Signals;
using UnityEngine;
using Zenject;

public class Laser : IInitializable, IDisposable
{
    private SignalBus _signalBus;
    private float _maxDistance;
    private Transform _shootPoint;
    private LineRenderer _lineRenderer;
    private float _laserDuration;
    private LayerMask _layerMaskIgnore;
    private float _cooldown;
    private float _lastShootTime;
    private float _currentAmmoCount;
    private float _maxAmmoCount;
    private float _ammoReloadCooldown;

    private CancellationTokenSource _shootCts;
    private CancellationTokenSource _reloadCts;
    

    private readonly RaycastHit2D[] _hitsBuffer = new RaycastHit2D[10];

    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _signalBus = signalBus;
        _lineRenderer = player.LineRenderer;
        _maxDistance = player.MaxRayDistance;
        _shootPoint = player.ShootPoint;
        _laserDuration = player.LaserDuration;
        _layerMaskIgnore = player.LayerMaskIgnore;
        _cooldown = player.LaserCooldown;
        _maxAmmoCount = player.MaxLaserAmmoCount;
        _currentAmmoCount = _maxAmmoCount;
        _ammoReloadCooldown = player.LaserReloadCooldown;
        _reloadCts = new CancellationTokenSource();
        
        if (_lineRenderer != null) 
            _lineRenderer.enabled = false;

        _lastShootTime = -_cooldown; 
    }

    private void ShootLaser()
    {
        ShootLaserRoutine().Forget();
    }

    private async UniTaskVoid ShootLaserRoutine()
    {
        if (Time.time < _lastShootTime + _cooldown || _currentAmmoCount == 0)
            return;

        if (_shootCts != null)
        {
            _shootCts.Cancel();
            _shootCts.Dispose();
        }

        _currentAmmoCount = Mathf.Clamp(_currentAmmoCount - 1, 0, _maxAmmoCount);
        ReloadLaserAmmo().Forget();
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
        }
        catch (OperationCanceledException)
        { 
        }
        finally
        {
            if (_lineRenderer != null)
                _lineRenderer.enabled = false;

            if (_shootCts != null)
            {
                _shootCts.Dispose();
                _shootCts = null;
            }
        }
    }

    private async UniTaskVoid ReloadLaserAmmo()
    {
        float elapsedTIme = 0;
        
        while (elapsedTIme < _ammoReloadCooldown)
        {
            elapsedTIme += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, _reloadCts.Token);
        }
        
        _currentAmmoCount = Mathf.Clamp(_currentAmmoCount + 1, 0, _maxAmmoCount);
    }

    private void UpdateLaser()
    {
        Vector2 origin = _shootPoint.position;
        Vector2 direction = _shootPoint.up;

        int hitCount = Physics2D.RaycastNonAlloc(origin, direction, _hitsBuffer, _maxDistance, ~_layerMaskIgnore);

        Vector2 endPoint;

        if (hitCount > 0)
        {
            float maxDistFound = -1f;
            Vector2 furthestPoint = origin;

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hitsBuffer[i];

                if (hit.distance > maxDistFound)
                {
                    maxDistFound = hit.distance;
                    furthestPoint = hit.point;
                }

                hit.collider.GetComponent<IDestroyable>()?.Destroy(DestroyReason.Shootable);
            }

            endPoint = furthestPoint;
        }
        else
        {
            endPoint = origin + (direction * _maxDistance);
        }

        _lineRenderer.SetPosition(0, new Vector3(origin.x, origin.y, -0.1f));
        _lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, -0.1f));
    }

    public void Initialize()
    {
        _signalBus.Subscribe<LaserShootSignal>(ShootLaser);
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<LaserShootSignal>(ShootLaser);
        _shootCts?.Cancel();
        _shootCts?.Dispose();
        _reloadCts?.Cancel();
        _reloadCts?.Dispose();
    }
}