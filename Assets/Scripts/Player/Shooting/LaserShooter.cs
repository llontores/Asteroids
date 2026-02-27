using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Signals;
using UnityEngine;
using UnityEngine.Events;
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
    private float _currentAmmoCount;
    private float _maxAmmoCount;
    private float _ammoReloadCooldown;
    private Player _player;
    private CancellationTokenSource _shootCts;
    private CancellationTokenSource _reloadCts;
    private readonly RaycastHit2D[] _hitsBuffer = new RaycastHit2D[10];
    private Camera _camera;
    
    // Границы экрана в мировых координатах
    private float _minX, _maxX, _minY, _maxY;

    public event UnityAction<Vector3> LaserEndPointUpdated;
    public event UnityAction LaserTurnedOff;
    
    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _player = player;
        _signalBus = signalBus;
    }

    private void ShootLaser() => ShootLaserRoutine().Forget();

    private async UniTaskVoid ShootLaserRoutine()
    {
        if (Time.time < _lastShootTime + _cooldown || _currentAmmoCount == 0 || _player.IsInvulnerable)
            return;

        if (_shootCts != null) { _shootCts.Cancel(); _shootCts.Dispose(); }

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
            LaserTurnedOff?.Invoke();
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
        Vector2 direction = _shootPoint.up; // Стреляем "вверх" от дула

        // 1. Находим потенциальную конечную точку лазера
        Vector2 targetEnd = origin + direction * _maxDistance;

        // 2. Ограничиваем эту точку границами экрана
        // Чтобы луч не "ломался", мы вычисляем, какой множитель (t) нужен, чтобы коснуться края
        float t = 1f;

        if (targetEnd.x < _minX) t = Mathf.Min(t, (_minX - origin.x) / (targetEnd.x - origin.x));
        if (targetEnd.x > _maxX) t = Mathf.Min(t, (_maxX - origin.x) / (targetEnd.x - origin.x));
        if (targetEnd.y < _minY) t = Mathf.Min(t, (_minY - origin.y) / (targetEnd.y - origin.y));
        if (targetEnd.y > _maxY) t = Mathf.Min(t, (_maxY - origin.y) / (targetEnd.y - origin.y));

        // Обновленная дистанция с учетом краев экрана
        float clampedDistance = _maxDistance * t;

        // 3. Физическая проверка (Raycast)
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
        
        LaserEndPointUpdated?.Invoke(renderPoint);
    }

    public void Initialize()
    {
        _signalBus.Subscribe<LaserShootSignal>(ShootLaser);
        LaserTurnedOff?.Invoke();
        _lineRenderer = _player.LineRenderer;
        _maxDistance = _player.Config.MaxRayDistance;
        _shootPoint = _player.ShootPoint;
        _laserDuration = _player.Config.LaserDuration;
        _layerMaskIgnore = _player.LayerMaskIgnore;
        _cooldown = _player.Config.LaserCooldown;
        _maxAmmoCount = _player.Config.MaxLaserAmmoCount;
        _currentAmmoCount = _maxAmmoCount;
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
        _signalBus.TryUnsubscribe<LaserShootSignal>(ShootLaser);
        _shootCts?.Cancel(); _shootCts?.Dispose();
        _reloadCts?.Cancel(); _reloadCts?.Dispose();
    }
}