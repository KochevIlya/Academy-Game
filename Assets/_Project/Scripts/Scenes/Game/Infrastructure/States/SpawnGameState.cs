using System;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Components.Spawner;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Scenes.Game.Infrastructure.States
{
  public class SpawnGameState : IEnterState
  {
    private readonly IGameFactory _gameFactory;
    public SpawnGameState(IGameFactory gameFactory)
    {
      _gameFactory = gameFactory;
    }
    
    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
      foreach (UnitSpawner spawner in Object.FindObjectsOfType<UnitSpawner>())
      {
        if(spawner.UnitType == UnitType.Character) 
          await _gameFactory.SpawnCharacter(spawner.Position, WeaponType.Riffle);
        else if (spawner.UnitType == UnitType.Bot)
          await _gameFactory.SpawnBot(spawner.Position, WeaponType.Riffle);
      }
      foreach (TerminalSpawner  spawner in Object.FindObjectsOfType<TerminalSpawner>())
        await _gameFactory.SpawnTerminal(spawner.Position);
      gameStateMachine.Enter<InitializeGameServices>();
    }
  }
}