using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.StateMachine.States.Interfaces
{
  public interface IExitState : IState
  {
    public UniTask Exit(IGameStateMachine gameStateMachine);
  }
}