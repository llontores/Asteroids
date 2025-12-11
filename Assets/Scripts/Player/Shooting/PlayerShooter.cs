using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Signals;
using UnityEngine;
using Zenject;

public class PlayerShooter : IInitializable, IDisposable
{
    private SignalBus _signalBus;
    private Transform _shootPoint;
    private int _bulletShootCooldown;
    private float _laserShootCooldown;
    private Bullet _bulletPrefab;
    private bool _canShootBullets = true;
    public ObjectPool<Bullet> BulletPool {get; private set; }

    [Inject]
    public void Construct(Player player, SignalBus signalBus, BulletsContainer  bulletsContainer)
    {
        _signalBus = signalBus;
        _shootPoint = player.ShootPoint;
        _bulletShootCooldown = player.BulletsShootCooldown;
        _bulletPrefab = player.BulletPrefab;
        BulletPool = new ObjectPool<Bullet>(10, _bulletPrefab, bulletsContainer.transform);
    }
    
    public void Initialize()
    {
        _signalBus.Subscribe<BulletShootSignal>(FireBullets);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<BulletShootSignal>(FireBullets);
    }

    private void FireBullets()
    {
        if (_canShootBullets == false)
            return;

        if (BulletPool.TryGetObject(out Bullet bullet))
        {
            bullet.transform.position = _shootPoint.position;
            bullet.transform.rotation = _shootPoint.rotation;
            bullet.gameObject.SetActive(true);
            bullet.OnBulletDestroyed += ReturnBulletToPool;
        }
        
        _canShootBullets =  false;
        BulletsCooldown();
    }

    private async UniTaskVoid BulletsCooldown()
    {
        await UniTask.Delay(_bulletShootCooldown);
        _canShootBullets = true;
    }
    
    private void ReturnBulletToPool(Bullet bullet)
    {
        bullet.OnBulletDestroyed -= ReturnBulletToPool;
        BulletPool.ReturnObject(bullet);
    }
}