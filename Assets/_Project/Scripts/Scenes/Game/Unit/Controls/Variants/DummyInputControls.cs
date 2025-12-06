using System;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Controls
{
  public class DummyInputControls : IInputControls
  {
    public IObservable<Vector2> OnMovement { get; } = new Subject<Vector2>();
    public IObservable<UniRx.Unit> OnShoot { get; } = new Subject<UniRx.Unit>();
    public IObservable<UniRx.Unit> OnAbilityUse { get; } = new Subject<UniRx.Unit>();
    public Vector2 MousePosition { get; } = new();
    
    public void Initialize()
    {
      
    }
  }
}