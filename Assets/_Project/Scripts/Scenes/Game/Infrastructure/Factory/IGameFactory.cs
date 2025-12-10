using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Infrastructure.Factory
{
  public interface IGameFactory
  {
    GameUnit SpawnCharacter(Vector3 position);
    GameUnit SpawnBot(Vector3 position);
  }
}