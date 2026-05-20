using UnityEngine;
using Zenject;
using MVVM;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private Player _player;
    [SerializeField] private MobileButtonsHandler _mobileButtonsHandler;
    [SerializeField] private HazardSpawnerReferences _hazardSpawnerReferences;
    [SerializeField] private LaserParticlesHandler _laserParticlesHandler;
    [SerializeField] private BulletsContainer _bulletsContainer;
    [SerializeField] private ExplosionParticlesPool _explosionParticlesPool;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private GameOverScreen _gameOverScreen;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private PlayerReferences _playerReferences;
    
    private PlayerConfig _playerConfig;
    private BulletConfig _bulletConfig;
    private AsteroidConfig _asteroidConfig;
    private FragmentConfig _fragmentConfig;
    private UFOConfig _ufoConfig;
    private HazardSpawnerConfig _hazardSpawnerConfig;
    private BulletsShooterConfig _bulletsShooterConfig;
    private ExplosionParticlesPoolConfig _explosionParticlesPoolConfig;
    
    public override void InstallBindings()
    {
        _playerConfig = JsonConfigLoader.LoadFromResources<PlayerConfig>("Configs/player_config");
        _bulletConfig = JsonConfigLoader.LoadFromResources<BulletConfig>("Configs/bullet_config");
        _asteroidConfig = JsonConfigLoader.LoadFromResources<AsteroidConfig>("Configs/asteroid_config");
        _fragmentConfig = JsonConfigLoader.LoadFromResources<FragmentConfig>("Configs/fragment_config");
        _ufoConfig =  JsonConfigLoader.LoadFromResources<UFOConfig>("Configs/ufo_config");
        _hazardSpawnerConfig = JsonConfigLoader.LoadFromResources<HazardSpawnerConfig>("Configs/hazardSpawner_config");
        _bulletsShooterConfig = JsonConfigLoader.LoadFromResources<BulletsShooterConfig>("Configs/bulletsShooter_config");
        _explosionParticlesPoolConfig = JsonConfigLoader.LoadFromResources<ExplosionParticlesPoolConfig>("Configs/explosionParticlesPool_config");
        
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<AccelerationSignal>();
        Container.DeclareSignal<TurnSignal>();
        Container.DeclareSignal<BulletShootSignal>();
        Container.DeclareSignal<LaserShootSignal>();
        Container.DeclareSignal<DestroyableDiedSignal>();
        Container.DeclareSignal<LaserReloadRemainingTimeChangedSignal>();
        Container.DeclareSignal<LaserRemainingAmmoCountUpdatedSignal>();
        Container.DeclareSignal<ScoreChangedSignal>();
        Container.DeclareSignal<PlayerSpeedChangedSignal>();
        Container.DeclareSignal<LaserEndPointUpdatedSignal>();
        Container.DeclareSignal<LaserTurnedOffSignal>();
        Container.DeclareSignal<PlayerDeadSignal>();
        Container.DeclareSignal<RestartButtonPressedSignal>();

        Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
        Container.Bind<BulletConfig>().FromInstance(_bulletConfig).AsSingle();
        Container.Bind<AsteroidConfig>().FromInstance(_asteroidConfig).AsSingle();
        Container.Bind<FragmentConfig>().FromInstance(_fragmentConfig).AsSingle();
        Container.Bind<UFOConfig>().FromInstance(_ufoConfig).AsSingle();
        Container.Bind<BulletsShooterConfig>().FromInstance(_bulletsShooterConfig).AsSingle();
        Container.Bind<HazardSpawnerConfig>().FromInstance(_hazardSpawnerConfig).AsSingle();
        Container.Bind<ExplosionParticlesPoolConfig>().FromInstance(_explosionParticlesPoolConfig).AsSingle();
        
        Container.BindInterfacesAndSelfTo<Player>().FromInstance(_player).AsSingle().NonLazy(); 
        Container.Bind<MobileButtonsHandler>().FromInstance(_mobileButtonsHandler).AsSingle();
        Container.BindInterfacesAndSelfTo<RewardCounter>().AsSingle().NonLazy();
        Container.Bind<HazardSpawnerReferences>().FromInstance(_hazardSpawnerReferences).AsSingle();
        Container.BindFactory<Asteroid, Asteroid.Factory>()
            .FromComponentInNewPrefab(_hazardSpawnerReferences.AsteroidPrefab);
        Container.BindFactory<UFO, UFO.Factory>()
            .FromComponentInNewPrefab(_hazardSpawnerReferences.UfoPrefab);
        Container.BindFactory<Fragment, Fragment.Factory>()
            .FromComponentInNewPrefab(_hazardSpawnerReferences.FragmentPrefab);
        Container.BindFactory<Bullet, Bullet.Factory>()
            .FromComponentInNewPrefab(_playerReferences.BulletPrefab);
        Container.BindInterfacesAndSelfTo<PlayerMover>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerEffectsController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScreenWrapper>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<BulletsShooter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LaserParticlesHandler>().FromInstance(_laserParticlesHandler).AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerFacade>().AsSingle().NonLazy();
        Container.Bind<BulletsContainer>().FromInstance(_bulletsContainer).AsSingle();
        Container.BindInterfacesAndSelfTo<ExplosionParticlesPool>().FromInstance(_explosionParticlesPool).AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerViewModel>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<Joystick>().FromInstance(_joystick).AsSingle();
        Container.BindInterfacesAndSelfTo<GameOverScreen>().FromInstance(_gameOverScreen).AsSingle();
        Container.BindInterfacesAndSelfTo<GameLauncher>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LaserShooter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AdsService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AnalyticsTracker>().AsSingle().NonLazy();
        Container.Bind<AsteroidFacade>().AsTransient();
        Container.Bind<FragmentFacade>().AsTransient();
        Container.Bind<UFOFacade>().AsTransient();
        Container.Bind<BulletFacade>().AsTransient();
        Container.BindInterfacesAndSelfTo<HazardSpawnerController>().AsSingle().NonLazy();
        Container.Bind<Camera>().FromInstance(_mainCamera).AsSingle();
        Container.Bind<PlayerReferences>().FromInstance(_playerReferences).AsSingle();
        Container.Bind<TargetsRegistry>().AsSingle();
        Container.Bind<CustomRaycaster>().AsSingle();

        BinderFactory.RegisterBinder<TextBinder>();

        Container.BindInitializableExecutionOrder<PlayerViewModel>(-100);
        Container.BindInitializableExecutionOrder<LaserShooter>(0);
        Container.BindInitializableExecutionOrder<RewardCounter>(1);
        
        if (SystemInfo.deviceType == DeviceType.Handheld)
            Container.BindInterfacesAndSelfTo<MobileInput>().AsSingle();
        else
        {
            Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle();
            _mobileButtonsHandler.gameObject.SetActive((false));
        }
        
    }
}