using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
  [SerializeField] private GameUnit _gameUnit;
  
  private void Start()
  {
    _gameUnit.UpdateControls(
      inputControls: new UserInputControls(), 
      mover: new MainCharacterMover(), 
      rotator: new MainCharacterRotator());
  }
}
