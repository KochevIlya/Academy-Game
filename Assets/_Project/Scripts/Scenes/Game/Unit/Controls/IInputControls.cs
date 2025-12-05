using System;

using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public interface IInputControls
  {
    IObservable<Vector2> OnMovement { get; }
    IObservable<Vector2> OnRotation { get; }
  }
}