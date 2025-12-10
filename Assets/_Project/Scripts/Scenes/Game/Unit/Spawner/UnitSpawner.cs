using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Spawner
{
  public class UnitSpawner : MonoBehaviour
  {
    public UnitType UnitType;
    public Vector3 Position => transform.position;
  }
}