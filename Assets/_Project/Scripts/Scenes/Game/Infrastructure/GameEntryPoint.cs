using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Attacker;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
  [SerializeField] private GameUnit _player;
  [SerializeField] private GameUnit _enemy;

  [SerializeField] private CameraService _cameraService;
  
  private void Start()
  {
    UserInputControls playerInputControls = new UserInputControls();
    playerInputControls.Initialize();

    _player.UpdateControls(
      inputControls: playerInputControls,
      mover: new MainCharacterMover(_cameraService),
      unitAttacker: new MainCharacterAttacker(_cameraService),
      unitRotator: new MainCharacterRotator(playerInputControls, _cameraService, _player));
    
    _enemy.UpdateControls(      
      inputControls: new DummyInputControls(), 
      mover: new MainCharacterMover(_cameraService), 
      unitAttacker: new MainCharacterAttacker(_cameraService),
      unitRotator: new MainCharacterRotator(playerInputControls, _cameraService, _enemy));
    
    _cameraService.SetTarget(_player);
  }
}
