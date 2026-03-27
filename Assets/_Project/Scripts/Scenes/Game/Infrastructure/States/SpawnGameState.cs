using System;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Components.Spawner;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Scenes.Game.Infrastructure.States
{
  public class SpawnGameState : IEnterState
  {
    private readonly IGameFactory _gameFactory;
    private readonly DiContainer _container;
    private readonly IProgressService _progressService;
    private readonly IGuiGameService _guiService;
    public SpawnGameState(IGameFactory gameFactory
      ,DiContainer container
      ,IProgressService progressService
      ,IGuiGameService guiService
    
    )
    {
      _container = container;
      _gameFactory = gameFactory;
      _progressService = progressService;
      _guiService = guiService;
    }
    
    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
      await UniTask.Yield();
      bool isNewGame = _progressService.Progress == null || _progressService.Progress.enemies.Count == 0;
      
      if (isNewGame)
      {
        
        foreach (UnitSpawner spawner in Object.FindObjectsOfType<UnitSpawner>())
        {
          GameUnit unit = await _gameFactory.SpawnGameUnit(spawner.Position, spawner.UnitСharacteristicsType, spawner.Path);
          if (unit != null)
          {
            spawner.SetSpawnedUnit(unit);
          }
        }
      }
      
      foreach (TerminalSpawner spawner in Object.FindObjectsOfType<TerminalSpawner>())
      {
        var terminal = await _gameFactory.SpawnTerminal(spawner.Position, spawner.WarZoneTransform, spawner.SpawnerId);
    
        if (terminal != null)
        {
          spawner.SpawnedTerminalObject = terminal.gameObject;
        }
      }
      
      foreach (var zone in Object.FindObjectsOfType<CombatZone>())
      {
        _container.Inject(zone);
        zone.InitializeZone();
        
      }
      
      gameStateMachine.Enter<InitializeGameServices>();
    }
  }
}