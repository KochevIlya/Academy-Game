using _Project.Scripts.Infrastructure.EntryPoint;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using _Project.Scripts.Utils.Extensions;
using Zenject;

namespace _Project.Scripts.Infrastructure.Installers
{
  public class ProjectEntryPointInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      SignalBusInstaller.Install(Container);
      Container.DeclareSignal<RestartLevelSignal>();
      
      Container.BindEntryPoint<ProjectEntryPoint>();
      
      Container.BindState<ReloadCurrentSceneState>();
      Container.BindState<LoadProjectState>();
      Container.BindState<MainMenuState>();
      Container.BindState<ExitToMainMenuState>();
      Container.BindState<InitializeCurrentSceneState>();
      
      Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
      
      Container.BindSignal<RestartLevelSignal>()
        .ToMethod<IGameStateMachine>(x => x.OnRestartRequested)
        .FromResolve();
    }
  }
}
