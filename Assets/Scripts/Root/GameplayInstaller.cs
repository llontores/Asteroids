using UnityEngine;
using Zenject;
using MVVM;

public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<AccelerationSignal>();
        Container.DeclareSignal<TurnSignal>();
        Container.DeclareSignal<BulletShootSignal>();
        Container.DeclareSignal<LaserShootSignal>();
        Container.DeclareSignal<DestroyableDiedSignal>();
        Container.DeclareSignal<LaserReloadRemainingTimeChangedSIgnal>();
        Container.DeclareSignal<LaserRemainingAmmoCountUpdatedSignal>();
        Container.DeclareSignal<ScoreChangedSignal>();
        Container.DeclareSignal<PlayerSpeedChangedSignal>();
        Container.DeclareSignal<LaserEndPointUpdatedSignal>();
        Container.DeclareSignal<LaserTurnedOffSignal>();
        Container.DeclareSignal<PlayerDeadSignal>();
        Container.DeclareSignal<RestartButtonPressedSignal>();
        Container.DeclareSignal<ShowGameOverUISignal>();

        Container.BindInterfacesAndSelfTo<Player>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<MobileButtonsHandler>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<RewardCounter>().AsSingle().NonLazy();
        Container.Bind<HazardSpawnerView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerMover>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerView>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScreenWrapper>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<BulletsShooter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LaserView>().FromComponentsInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerModel>().AsSingle().NonLazy();
        Container.Bind<BulletsContainer>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WorldSpace>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ExplosionParticlesPool>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerViewModel>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<Joystick>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<GameOverScreen>().FromComponentsInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<GameLauncher>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LaserShooter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AdManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AnalyticsManager>().AsSingle().NonLazy();
        Container.Bind<AsteroidModel>().AsTransient();
        Container.Bind<FragmentModel>().AsTransient();
        Container.Bind<UFOModel>().AsTransient();
        Container.BindInterfacesAndSelfTo<HazardSpawnerController>().AsSingle().NonLazy();

        BinderFactory.RegisterBinder<TextBinder>();

        Container.BindInitializableExecutionOrder<PlayerViewModel>(-100);
        Container.BindInitializableExecutionOrder<LaserShooter>(0);
        Container.BindInitializableExecutionOrder<RewardCounter>(1);
        
        if (SystemInfo.deviceType == DeviceType.Handheld)
            Container.BindInterfacesAndSelfTo<MobileInput>().AsSingle();
        else
            Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle();
        
    }
}