using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using _Project.Scripts.Libs.Pool;

namespace _Project.Scripts.Scenes.Game.Infrastructure.Factory
{
  public interface IGameFactory
  {
    UniTask<GameUnit> SpawnCharacter(Vector3 position, WeaponType weapon);
    UniTask<GameUnit> SpawnBot(Vector3 position, WeaponType weapon);
    UniTask<WeaponBase> SpawnWeapon(WeaponType weaponType, GameUnit unit);
    UniTask<Bullet> SpawnBullet(AssetReference prefabRefence, Transform spawnPoint);

    UniTask Initialize(AssetReference prefabReference);
  }
}