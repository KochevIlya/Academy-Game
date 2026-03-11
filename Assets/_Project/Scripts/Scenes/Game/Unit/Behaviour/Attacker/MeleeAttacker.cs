using UnityEngine;
using Zenject;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Attacker;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;


public class MeleeAttacker : MonoBehaviour, IUnitAttacker
{
    private Vector3 _shootMousePosition;
    private IInputHelper _inputHelper;
    private UserInputControls _userInputControls;
    private const float DefaultFireHeight = 1.2f;
    
    [Inject]
    public void Construct(UserInputControls userInputControls,IInputHelper inputHelper)
    {
        _userInputControls = userInputControls;
        _inputHelper = inputHelper;
    }

    
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
        if (unit.Ability != null && unit.Ability.CanUse())
        {
            unit.Ability.Use(new Vector3());
        }
        
    }
    
    
}
