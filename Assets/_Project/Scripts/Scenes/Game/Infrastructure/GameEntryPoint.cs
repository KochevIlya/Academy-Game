using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
  [SerializeField] private GameUnit _gameUnit;
  
  private void Start()
  {
    UserInputControls inputControls = new UserInputControls();
    
    _gameUnit.UpdateControls(
      inputControls: inputControls, 
      mover: new MainCharacterMover(), 
      rotator: new MainCharacterRotator());
    
    inputControls.Initialize();
  }
}
