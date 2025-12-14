using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Attacker
{
  public interface IUnitAttacker
  {
    void Shoot(GameUnit unit, Vector2 shootPosition);
    void OnShootCast(GameUnit unit);
    void AbilityUse(GameUnit unit);
  }
}