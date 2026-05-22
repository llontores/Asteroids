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
    
    public override void InstallBindings()
    {
        ConfigProvider configProvider = new ConfigProvider();
        configProvider.LoadAll();
        
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
        
        Container.Bind<PlayerConfig>().FromInstance(configProvider.Player).AsSingle();
        Container.Bind<BulletConfig>().FromInstance(configProvider.Bullet).AsSingle();
        Container.Bind<AsteroidConfig>().FromInstance(configProvider.Asteroid).AsSingle();
        Container.Bind<FragmentConfig>().FromInstance(configProvider.Fragment).AsSingle();
        Container.Bind<UFOConfig>().FromInstance(configProvider.UFO).AsSingle();
        Container.Bind<BulletsShooterConfig>().FromInstance(configProvider.BulletsShooter).AsSingle();
        Container.Bind<HazardSpawnerConfig>().FromInstance(configProvider.HazardSpawner).AsSingle();
        Container.Bind<ExplosionParticlesPoolConfig>().FromInstance(configProvider.ExplosionParticlesPool).AsSingle();
        
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
        
        Container.Bind<FragmentsPool>().AsSingle().WithArguments(configProvider.HazardSpawner.FragmentsCapacity, _hazardSpawnerReferences.FragmentContainer);
        Container.BindInterfacesAndSelfTo<HazardSpawnerController>().AsSingle().NonLazy();
        
        Container.Bind<Camera>().FromInstance(_mainCamera).AsSingle();
        Container.Bind<PlayerReferences>().FromInstance(_playerReferences).AsSingle();
        Container.Bind<TargetsRegistry>().AsSingle();
        Container.Bind<CustomRaycaster>().AsSingle();

        BinderFactory.RegisterBinder<TextBinder>();
        BinderFactory.RegisterBinder<HeartsBinder>();

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