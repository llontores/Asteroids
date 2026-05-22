using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BulletsShooter : IInitializable, IDisposable
{
    private SignalBus _signalBus;
    private Transform _shootPoint;
    private int _bulletShootCooldown;
    private Bullet _bulletPrefab;
    private bool _canShootBullets = true;
    private CancellationTokenSource _cancellationToken;
    private BulletsShooterConfig _shooterConfig;
    private int _poolCapacity;
    private PlayerConfig _playerConfig;
    private PlayerReferences _playerReferences;
    private PlayerFacade _playerFacade;
    private BulletsContainer _bulletsContainer;
    private Bullet.Factory _bulletFactory;

    public ObjectPool<Bullet> BulletPool { get; private set; }

    [Inject]
    public void Construct(PlayerFacade playerFacade, SignalBus signalBus, BulletsContainer bulletsContainer,
        BulletsShooterConfig shooterConfig, PlayerConfig playerConfig, PlayerReferences playerReferences,
        Bullet.Factory bulletFactory)
    {
        _playerConfig = playerConfig;
        _shooterConfig = shooterConfig;
        _playerFacade = playerFacade;
        _signalBus = signalBus;
        _playerReferences = playerReferences;
        _bulletsContainer = bulletsContainer;
        _bulletPrefab = playerReferences.BulletPrefab;
        _bulletFactory = bulletFactory;
    }

    public void Initialize()
    {
        _poolCapacity = _shooterConfig.PoolCapacity;
        BulletPool = new ObjectPool<Bullet>(_poolCapacity, _bulletFactory, _bulletsContainer.transform);
        _bulletPrefab = _playerReferences.BulletPrefab;
        _signalBus.Subscribe<BulletShootSignal>(FireBullets);
        _shootPoint = _playerReferences.ShootPoint;
        _bulletShootCooldown = _playerConfig.BulletsShootCooldown;
        _cancellationToken = new CancellationTokenSource();
        _signalBus.Subscribe<PlayerDeadSignal>(StopCooldown);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<PlayerDeadSignal>(StopCooldown);
        _signalBus.Unsubscribe<BulletShootSignal>(FireBullets);
        _cancellationToken.Cancel();
        _cancellationToken.Dispose();
    }

    private void FireBullets()
    {
        if (_canShootBullets == false || _playerFacade.IsInvulnerable)
            return;

        if (BulletPool.TryGetObject(out Bullet bullet))
        {
            bullet.transform.position = _shootPoint.position;
            bullet.transform.rotation = _shootPoint.rotation;
            bullet.gameObject.SetActive(true);
            bullet.OnBulletDestroyed += ReturnBulletToPool;
            _canShootBullets = false;
            BulletsCooldown().Forget();
        }
    }

    public void StopCooldown()
    {
        _cancellationToken.Cancel();
        _cancellationToken.Dispose();
        _cancellationToken = new CancellationTokenSource(); 
        _canShootBullets = true;
    }

    private async UniTask BulletsCooldown()
    {
        await UniTask.Delay(_bulletShootCooldown, cancellationToken: _cancellationToken.Token);
        _canShootBullets = true;
    }

    private void ReturnBulletToPool(Bullet bullet)
    {
        bullet.OnBulletDestroyed -= ReturnBulletToPool;
        BulletPool.ReturnObject(bullet);
    }
}