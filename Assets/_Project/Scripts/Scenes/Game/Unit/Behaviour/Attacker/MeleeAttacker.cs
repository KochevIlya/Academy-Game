using UnityEngine;
using Zenject;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Attacker;


public class MeleeAttacker : MonoBehaviour, IUnitAttacker
{
    public void Attack(GameUnit unit, Vector2 shootPosition) 
    {
        if (unit.HasWeapon)
        {
            unit.Animator.Shoot(); 
        }
    }

    public void OnShootCast(GameUnit unit)
    {
        if (unit.HasWeapon)
        {
            unit.Weapon.Shoot(Vector2.zero, unit);
        }
    }

    public void AbilityUse(GameUnit unit)
    {
        Debug.Log($"[{unit.name}] Melee Ability Use");
    }
    
    
}
