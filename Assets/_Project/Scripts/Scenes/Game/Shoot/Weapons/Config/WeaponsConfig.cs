using System.Collections.Generic;
using _Project.Scripts.Libs.Configs.Variants;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Shoot.Config
{ 
  [CreateAssetMenu(menuName = "Configs/" + nameof(WeaponsConfig), fileName = nameof(WeaponsConfig))]
  public class WeaponsConfig : SoConfig<WeaponsConfig>
  {
    public Dictionary<WeaponType, WeaponData> Weapons;
    public static float BulletLifeTime = 3f;
  }
}