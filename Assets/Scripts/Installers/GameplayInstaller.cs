using Signals;
using UnityEngine;
using Zenject;

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

        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MobileButtonsHandler>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<RewardCounter>().AsSingle().NonLazy();
        Container.Bind<HazardSpawner>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerMover>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerView>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScreenWrapper>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<BulletsShooter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LaserShooter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LaserView>().FromComponentsInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerModel>().AsSingle().NonLazy();
        Container.Bind<BulletsContainer>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WorldSpace>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ExplosionParticlesPool>().FromComponentInHierarchy().AsSingle();

        if (SystemInfo.deviceType == DeviceType.Handheld)
            Container.BindInterfacesAndSelfTo<MobileInput>().AsSingle();
        else
            Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle();
    }
}