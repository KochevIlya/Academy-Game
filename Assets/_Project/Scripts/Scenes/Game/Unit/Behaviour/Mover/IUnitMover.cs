using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Mover
{
  public interface IUnitMover
  {
    
    void Move(GameUnit gameUnit, Vector2 movementDelta, float deltaTime);
    void ResetMovement(GameUnit gameUnit);
  }
}