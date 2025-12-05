using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Attacker;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
  [SerializeField] private GameUnit _player;
  [SerializeField] private GameUnit _enemy;
  
  private void Start()
  {
    UserInputControls playerInputControls = new UserInputControls();
    playerInputControls.Initialize();
    
    _player.UpdateControls(
      inputControls: playerInputControls, 
      mover: new MainCharacterMover(), 
      rotator: new MainCharacterRotator(),
      unitAttacker: new MainCharacterAttacker());
    
    _enemy.UpdateControls(      
      inputControls: playerInputControls, 
      mover: new MainCharacterMover(), 
      rotator: new MainCharacterRotator(),
      unitAttacker: new MainCharacterAttacker());
  }
}
