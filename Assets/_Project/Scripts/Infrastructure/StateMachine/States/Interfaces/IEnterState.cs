using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.StateMachine.States.Interfaces
{
  public interface IEnterState : IState
  {
    public UniTask Enter(IGameStateMachine gameStateMachine);
  }
}