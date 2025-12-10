using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Zenject;

namespace _Project.Scripts.Utils.Extensions
{
  public static class ZenjectExtensions
  {
    public static void BindState<T>(this DiContainer diContainer) where T : IState
    {
      diContainer.Bind<IState>().To<T>().AsSingle();
    }
    
    public static void BindEntryPoint<T>(this DiContainer diContainer) where T : IInitializable
    {
      diContainer.BindInitializableExecutionOrder<T>(-10);
      diContainer.Bind<IInitializable>().To<T>().AsSingle();
    }
  }
}