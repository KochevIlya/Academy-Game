using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.EntryPoint
{
  public class ProjectEntryPoint : IInitializable
  {
    private readonly IGameStateMachine _gameStateMachine;
    public ProjectEntryPoint(IGameStateMachine gameStateMachine)
    {
      _gameStateMachine = gameStateMachine;
    }
    
    public void Initialize()
    {
      _gameStateMachine.Enter<LoadProjectState>();
      
    }
  }
}