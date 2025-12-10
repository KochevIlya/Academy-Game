using _Project.Scripts.Infrastructure.StaticData;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Infrastructure.Factory
{

  public class GameFactory : IGameFactory
  {
    private readonly IStaticDataService _staticData;
    private readonly DiContainer _diContainer;
    private readonly UserInputControls _userInputControls;
    private readonly DummyInputControls _dummyInputControls;
    public GameFactory(IStaticDataService staticData, DiContainer diContainer, 
      UserInputControls userInputControls, DummyInputControls dummyInputControls)
    {
      _staticData = staticData;
      _diContainer = diContainer;
      _userInputControls = userInputControls;
      _dummyInputControls = dummyInputControls;
    }
    
    public GameUnit SpawnCharacter(Vector3 position)
    {
      GameUnit character = _diContainer
        .InstantiatePrefabForComponent<GameUnit>(_staticData.UnitsConfig.Character, 
          position, Quaternion.identity, null);
      
      character.UpdateControls(_userInputControls);
      return character;
    }

    public GameUnit SpawnBot(Vector3 position)
    {
      GameUnit bot = _diContainer
        .InstantiatePrefabForComponent<GameUnit>(_staticData.UnitsConfig.Character, 
          position, Quaternion.identity, null);
      
      bot.UpdateControls(_dummyInputControls);
      return bot;
    }
  }
}