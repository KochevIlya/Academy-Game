using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.StateMachine.States.Interfaces
{
  public interface IOverloadedEnterState<in T> : IState
  {
    public UniTask Enter(IGameStateMachine gameStateMachine, T overload);
  }
}