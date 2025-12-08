using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit.Attacker
{
  public class MainCharacterAttacker : MonoBehaviour, IUnitAttacker
  {
    private ICameraService _cameraService;

    [Inject]
    public void Construct(ICameraService cameraService)
    {
      _cameraService = cameraService;
    }

    public void Shoot(GameUnit unit)
    {
      unit.Animator.Shoot();
      Debug.Log("Start Shoot");
    }

    public void OnShootCast(GameUnit unit)
    {
      Debug.Log($"Shoot Cast with position: {unit.InputControls.MousePosition}");
    }

    public void AbilityUse(GameUnit unit)
    {
      foreach (var sceneUnit in FindObjectsOfType<GameUnit>())
      {
        if (sceneUnit != unit)
        {
          unit.UpdateControls(new DummyInputControls());
          var userInputControls = new UserInputControls();
          userInputControls.Initialize();
          sceneUnit.UpdateControls(userInputControls);
          _cameraService.SetTarget(sceneUnit);

          break;
        }
      }
    }
  }
}