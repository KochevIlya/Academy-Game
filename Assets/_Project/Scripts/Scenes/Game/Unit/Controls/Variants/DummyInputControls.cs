using System;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Controls
{
  public class DummyInputControls : IInputControls
  {
    private readonly Subject<Vector2> _movement = new();
    private readonly Subject<Vector2> _rotation = new();

    public IObservable<Vector2> OnMovement => _movement;
    public IObservable<Vector2> OnRotation => _rotation;
    public void Initialize()
    {
      
    }
  }
}