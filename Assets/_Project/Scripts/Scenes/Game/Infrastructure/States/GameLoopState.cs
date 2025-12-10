using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Scenes.Game.Infrastructure.States
{
  public class GameLoopState : IEnterState
  {
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
      return UniTask.CompletedTask;
    }
  }
}