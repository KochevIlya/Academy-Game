using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Infrastructure
{
  public class GameEntryPoint : IInitializable
  {
    private readonly IGameStateMachine _gameStateMachine;
    public GameEntryPoint(IGameStateMachine gameStateMachine)
    {
      _gameStateMachine = gameStateMachine;
    }
    
    public void Initialize()
    {
      Debug.Log("!!! GameEntryPoint Initialized !!!");
      _gameStateMachine.Enter<SpawnGameState>();
    }
  }
}