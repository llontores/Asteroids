using System;
using Signals;
using UnityEngine;
using Zenject;

public class Laser : IInitializable, IDisposable 
{
    private SignalBus _signalBus;
    private float _maxDistance;
    private Transform _shootPoint;
    private LineRenderer _lineRenderer;

    [Inject]
    public void Construct(SignalBus signalBus, Player player)
    {
        _signalBus = signalBus;
        _lineRenderer = player.LineRenderer;
        _maxDistance = player.MaxRayDistance;
        _shootPoint = player.ShootPoint; 
    }

    private void UpdateLaser()
    {
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

        _lineRenderer.SetPosition(0, new Vector3(origin.x,origin.y, -0.1f));   // Начало линии
        _lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, -0.1f)); // Конец линии
    }

    private void ShootLaser()
    {
        UpdateLaser();
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<LaserShootSignal>(ShootLaser);
    }

    public void Initialize()
    {
        _signalBus.Subscribe<LaserShootSignal>(ShootLaser);
    }
}
