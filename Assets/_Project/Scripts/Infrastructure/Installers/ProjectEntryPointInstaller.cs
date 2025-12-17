using _Project.Scripts.Infrastructure.EntryPoint;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using _Project.Scripts.Utils.Extensions;
using Zenject;

namespace _Project.Scripts.Infrastructure.Installers
{
  public class ProjectEntryPointInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindEntryPoint<ProjectEntryPoint>();

      Container.BindState<LoadProjectState>();
      Container.BindState<InitializeCurrentSceneState>();
      Container.BindState<InitializeGameServices>();
      Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
    }
  }
}