using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Mover
{
  public interface IUnitMover
  {
    
    void Move(GameUnit gameUnit, Vector3 movementDirection, float deltaTime, float speed);
    void ResetMovement(GameUnit gameUnit);
  }
}