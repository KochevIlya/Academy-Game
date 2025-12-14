using _Project.Scripts.Libs.Pool.Item;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public abstract class WeaponBase : MonoSpawnableItem
  {
    public Transform SpawnPoint;
    
    protected WeaponData WeaponData;
    protected GameUnit Unit;

    public abstract void Shoot(Vector2 shootMousePosition);
    public void Setup(WeaponData weaponData, GameUnit unit)
    {
      WeaponData = weaponData;
      Unit = unit;
    }
  }
}