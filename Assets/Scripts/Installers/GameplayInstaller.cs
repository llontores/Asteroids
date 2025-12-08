using DefaultNamespace;
using Inputs;
using Signals;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerMover>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerView>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScreenWrapper>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerShooter>().AsSingle().NonLazy();
        Container.Bind<BulletsContainer>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WorldSpace>().FromComponentInHierarchy().AsSingle();
        
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<AccelerationSignal>();
        Container.DeclareSignal<TurnSignal>();
        Container.DeclareSignal<BulletShootSignal>();
    }
}