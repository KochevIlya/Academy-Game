using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Spawner;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Infrastructure.States
{
  public class SpawnGameState : IEnterState
  {
    private readonly IGameFactory _gameFactory;
    public SpawnGameState(IGameFactory gameFactory)
    {
      _gameFactory = gameFactory;
    }
    
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
      foreach (UnitSpawner spawner in Object.FindObjectsOfType<UnitSpawner>())
      { 
        if(spawner.UnitType == UnitType.Character) 
          _gameFactory.SpawnCharacter(spawner.Position);
        else if (spawner.UnitType == UnitType.Bot)
          _gameFactory.SpawnBot(spawner.Position);
      }
      
      gameStateMachine.Enter<GameLoopState>();
      return UniTask.CompletedTask;
    }
  }
}