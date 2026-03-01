using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit.Attacker
{
  public class MainCharacterAttacker : MonoBehaviour, IUnitAttacker
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
        _shootMousePosition = shootPosition; 
        unit.Animator.Shoot();
      }
    }

    public void OnShootCast(GameUnit unit)
    {
      if(unit.HasWeapon) 
        unit.Weapon.Shoot(_shootMousePosition, unit);
    }
    
    public void AbilityUse(GameUnit unit)
    {
      // foreach (var sceneUnit in FindObjectsOfType<GameUnit>())
      // {
      //   if (sceneUnit != unit)
      //   {
      //     unit.UpdateControls(_dummyInputControls);
      //     sceneUnit.UpdateControls(_userInputControls);
      //     
      //     _cameraService.SetTarget(sceneUnit);
      //
      //     break;
      //   }
      // }
      if (unit.Ability != null && unit.Ability.CanUse())
      {
        Vector2 screenMousePos = _userInputControls.MousePosition;
        _inputHelper.ScreenToGroundPosition(screenMousePos, DefaultFireHeight, out var worldPosition);
        unit.Ability.Use(worldPosition);
      }
      Debug.Log($"[{unit.name}] Bot Ability Use");
    }
  }
}