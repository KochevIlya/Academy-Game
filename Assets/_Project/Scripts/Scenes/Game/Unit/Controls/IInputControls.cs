using System;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public interface IInputControls
  {
    void Initialize();
    IObservable<Vector2> OnMovement { get; }
    IObservable<Vector2> OnRotation { get; }
  }
}