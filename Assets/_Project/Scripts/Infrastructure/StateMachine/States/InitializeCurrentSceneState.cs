using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.StateMachine.States
{
  public class InitializeCurrentSceneState : IEnterState
  {
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
      Time.timeScale = 1f;
      
      Debug.Log("In InitializeCurrentSceneState");
      Object.FindAnyObjectByType<SceneContext>().Run();
      return UniTask.CompletedTask;
    }
  }
}