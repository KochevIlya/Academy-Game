using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Attacker
{
  public interface IUnitAttacker
  {
    void Shoot(Vector3 position);
    void AbilityUse();
  }
}