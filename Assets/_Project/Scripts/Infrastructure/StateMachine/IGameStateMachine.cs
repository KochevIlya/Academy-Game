using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Infrastructure.StateMachine
{
  public interface IGameStateMachine : ITickable, IFixedTickable
  {
    UniTask Enter<TState>() where TState : IState;
    UniTask Enter<TState, TOverload>(TOverload overload) where TState : IOverloadedEnterState<TOverload>;
  }
}