using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls
{
  public interface IInputHelper
  {
    bool ScreenToGroundPosition(Vector2 screenPosition, float groundHeight, out Vector3 groundPosition);
  }
}