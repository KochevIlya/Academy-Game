using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Rotator
{
  public interface IUnitRotator
  {
    void Rotate(GameUnit gameUnit, Vector2 movementDelta, float deltaTime);
  }
}