using _Project.Scripts.Libs.Pool.Item;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit;
using JetBrains.Annotations;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public abstract class WeaponBase : MonoSpawnableItem
  {
    public Transform SpawnPoint;
    
    protected WeaponData WeaponData;
    protected GameUnit Unit;
    
    [Header("VFX (to disable, leave nothing in prefab)")]
    [SerializeField] [CanBeNull] private GameObject VFXPrefab;
    [SerializeField] private Vector3 vfxOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float vfxScaleModifier = 1;

    public abstract void Shoot(Vector2 shootMousePosition, GameUnit unit);
    public void Setup(WeaponData weaponData, GameUnit unit)
    {
      WeaponData = weaponData;
      Unit = unit;
    }
    
    protected void PerformVFX()
    {
      if (VFXPrefab)
      {
        var vfxObj = Instantiate(VFXPrefab, transform.position + vfxOffset, Quaternion.identity);
        vfxObj.transform.localScale *= vfxScaleModifier;
      }
    }
  }
}