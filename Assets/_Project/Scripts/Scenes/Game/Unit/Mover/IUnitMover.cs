using System;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public interface IUnitMover
  {
    void Move(GameUnit gameUnit, Vector2 movementDelta);
  }
}