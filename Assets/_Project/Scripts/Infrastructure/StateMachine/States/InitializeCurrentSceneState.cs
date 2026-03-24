using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure;
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
      var sceneContext = Object.FindAnyObjectByType<SceneContext>();
      sceneContext.Run();
      var entryPoint = sceneContext.Container.Resolve<GameEntryPoint>();
      entryPoint.Run();
      
      return UniTask.CompletedTask;
    }
  }
}