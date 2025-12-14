using _Project.Scripts.Infrastructure.AssetProvider;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.StateMachine.States
{
  public class LoadProjectState : IEnterState
  {
    private readonly IStaticDataService _staticData;
    private readonly IAssetProvider _assetProvider;
    public LoadProjectState(IStaticDataService staticData, IAssetProvider assetProvider)
    {
      _staticData = staticData;
      _assetProvider = assetProvider;
    }
    
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
      _staticData.LoadAll();
      _assetProvider.Initialize();

      gameStateMachine.Enter<InitializeCurrentSceneState>();
      return UniTask.CompletedTask;
    }
  }
}