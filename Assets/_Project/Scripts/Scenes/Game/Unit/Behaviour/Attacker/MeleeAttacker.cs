using UnityEngine;
using Zenject;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Attacker;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Sounds;


public class MeleeAttacker : MonoBehaviour, IUnitAttacker
{
    private Vector3 _shootMousePosition;
    private IInputHelper _inputHelper;
    private UserInputControls _userInputControls;
    private ISoundService _soundService;
    private const float DefaultFireHeight = 1.2f;
    
    [Inject]
    public void Construct(UserInputControls userInputControls,IInputHelper inputHelper
    ,ISoundService soundService
    )
    {
        _userInputControls = userInputControls;
        _soundService = soundService;
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
            _soundService.Stop(Audio.AudioType.Sword);
            _soundService.Play(Audio.AudioType.Sword);
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
