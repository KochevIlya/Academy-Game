using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit.Attacker
{
  public class MainCharacterAttacker : MonoBehaviour, IUnitAttacker
  {
    private ICameraService _cameraService;
    
    private Vector3 _shootMousePosition;
    private UserInputControls _userInputControls;
    private DummyInputControls _dummyInputControls;

    [Inject]
    public void Construct(ICameraService cameraService, 
      UserInputControls userInputControls, DummyInputControls dummyInputControls)
    {
      _dummyInputControls = dummyInputControls;
      _userInputControls = userInputControls;
      _cameraService = cameraService;
    }

    public void Shoot(GameUnit unit, Vector2 shootPosition)
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
        unit.Weapon.Shoot(_shootMousePosition);
    }
    
    public void AbilityUse(GameUnit unit)
    {
      foreach (var sceneUnit in FindObjectsOfType<GameUnit>())
      {
        if (sceneUnit != unit)
        {
          unit.UpdateControls(_dummyInputControls);
          sceneUnit.UpdateControls(_userInputControls);
          
          _cameraService.SetTarget(sceneUnit);

          break;
        }
      }
    }
  }
}