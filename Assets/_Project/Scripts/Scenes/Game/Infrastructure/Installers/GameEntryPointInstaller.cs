using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using _Project.Scripts.Utils.Extensions;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Infrastructure
{
  public class GameEntryPointInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindState<InitializeGameServices>();
      Container.BindState<SpawnGameState>();
      Container.BindState<GameLoopState>();
      Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
      
      Container.BindInterfacesAndSelfTo<GameEntryPoint>().AsSingle().NonLazy();
    }
  }
}