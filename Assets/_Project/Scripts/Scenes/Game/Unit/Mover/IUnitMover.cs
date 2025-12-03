using System;
using System.Numerics;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public interface IUnitMover
  {
    void Move(Vector2 movementDelta);
  }
}