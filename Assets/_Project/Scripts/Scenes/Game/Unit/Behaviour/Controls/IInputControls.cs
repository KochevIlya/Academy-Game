using System;
using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls
{
  public enum MoverType { Player, Bot, Dummy }
  public interface IInputControls
  {
    public Vector2 MousePosition { get; }
    IObservable<Vector3> OnMovement { get; }
    IObservable<UniRx.Unit> OnShoot { get; }
    IObservable<UniRx.Unit> OnAbilityUse { get; }
    IObservable<Vector2> OnRawMovement { get; }
    MoverType RequiredMoverType { get; }
    float GetMovementSpeed(UnitStatsData stats);
  }
}