using System;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public interface IInputControls
  {
    void Initialize();
    public Vector2 MousePosition { get; }
    IObservable<Vector2> OnMovement { get; }
    IObservable<UniRx.Unit> OnShoot { get; }
    IObservable<UniRx.Unit> OnAbilityUse { get; }
  }
}