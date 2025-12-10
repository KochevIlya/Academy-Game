using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.StateMachine.States
{
  public class LoadProjectState : IEnterState
  {
    private readonly IStaticDataService _staticData;
    public LoadProjectState(IStaticDataService staticData)
    {
      _staticData = staticData;
    }
    
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
      _staticData.LoadAll();

      gameStateMachine.Enter<InitializeCurrentSceneState>();
      return UniTask.CompletedTask;
    }
  }
}