using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Attacker
{
  public class MainCharacterAttacker : IUnitAttacker
  {
    public void Shoot(Vector3 position)
    {
      Debug.Log("Shoot");
    }
    
    public void AbilityUse()
    {
      Debug.Log("Ability use");
    }
  }
}