using System;
using System.Threading; // Нужно для CancellationToken
using Cysharp.Threading.Tasks; // Подключаем UniTask
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

    // Токен для отмены текущего выстрела (если выстрелим снова или объект умрет)
    private CancellationTokenSource _shootCts;

    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _signalBus = signalBus;
        _lineRenderer = player.LineRenderer;
        _maxDistance = player.MaxRayDistance;
        _shootPoint = player.ShootPoint; 
        _laserDuration = player.LaserDuration;
        
        // Сразу скрываем лазер при старте
        if(_lineRenderer != null) _lineRenderer.enabled = false;
    }

    // Этот метод запускается при получении сигнала
    private void ShootLaser()
    {
        // Запускаем асинхронную операцию и "забываем" о ней (fire and forget)
        ShootLaserRoutine().Forget();
    }

    private async UniTaskVoid ShootLaserRoutine()
    {
        // 1. Отменяем предыдущий выстрел, если он еще идет
        if (_shootCts != null)
        {
            _shootCts.Cancel();
            _shootCts.Dispose();
        }
        
        // Создаем новый токен для текущего выстрела
        _shootCts = new CancellationTokenSource();
        var token = _shootCts.Token;

        try
        {
            // 2. Включаем лазер
            _lineRenderer.enabled = true;

            float startTime = Time.time;

            // 3. Цикл: пока не прошло нужное время
            while (Time.time < startTime + _laserDuration)
            {
                UpdateLaser(); // Обновляем позицию луча

                // Ждем следующего кадра (аналог yield return null)
                // Передаем token, чтобы прервать ожидание, если выстрел отменили
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            // Сюда попадем, если выстрел был прерван новым выстрелом или Dispose
            // Можно ничего не делать, просто выходим
        }
        finally
        {
            // 4. Блок finally выполнится ВСЕГДА: и при успешном завершении, 
            // и при ошибке, и при отмене. Тут выключаем лазер.
            if (_lineRenderer != null) 
                _lineRenderer.enabled = false;
            
            // Очищаем токен
            if (_shootCts != null)
            {
                 _shootCts.Dispose();
                 _shootCts = null;
            }
        }
    }

    private void UpdateLaser()
    {
        // Этот код остается без изменений, он просто считает физику
        Vector2 origin = _shootPoint.position;
        Vector2 direction = _shootPoint.up; 

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, _maxDistance);

        Vector2 endPoint;
        if (hit.collider != null)
        {
            endPoint = hit.point;
        }
        else
        {
            endPoint = origin + (direction * _maxDistance);
        }

        _lineRenderer.SetPosition(0, new Vector3(origin.x,origin.y, -0.1f));
        _lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, -0.1f));
    }

    public void Initialize()
    {
        _signalBus.Subscribe<LaserShootSignal>(ShootLaser);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<LaserShootSignal>(ShootLaser);
        
        // ВАЖНО: Если игрок вышел или объект удаляется, отменяем активную задачу,
        // иначе UniTask продолжит работать и попытается обратиться к удаленному LineRenderer.
        if (_shootCts != null)
        {
            _shootCts.Cancel();
            _shootCts.Dispose();
        }
    }
}