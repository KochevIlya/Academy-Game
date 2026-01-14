using System;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Scenes.Game.Shoot.Data
{
  [Serializable]
  public class WeaponData
  {
    public float CoolDown;
    public float BulletLifeTime;
    public float Speed;
    public AssetReference Bullet;
    public AssetReference Prefab;
  }
}