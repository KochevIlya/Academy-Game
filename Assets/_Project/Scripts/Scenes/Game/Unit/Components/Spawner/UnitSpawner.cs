using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Spawner
{
  public class UnitSpawner : MonoBehaviour
  {
    public UnitType UnitType;
    public UnitСharacteristicsType UnitСharacteristicsType;
    public GameUnit SpawnedUnit { get; private set; }
    public Vector3 Position => transform.position;
    public void SetSpawnedUnit(GameUnit unit)
    {
      SpawnedUnit = unit;
    }
  }
}