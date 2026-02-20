using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using _Project.Scripts.Libs.Pool;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;

namespace _Project.Scripts.Scenes.Game.Infrastructure.Factory
{
  public interface IGameFactory
  {
    UniTask<GameUnit> SpawnCharacter(Vector3 position, WeaponType weapon);
    UniTask<GameUnit> SpawnBot(Vector3 position, WeaponType weapon, UnitСharacteristicsType unitСharacteristicsType, PatrolPath patrolPath);
    UniTask<WeaponBase> SpawnWeapon(WeaponType weaponType, GameUnit unit);
    UniTask<Bullet> SpawnBullet(AssetReference prefabRefence, Transform spawnPoint);
    UniTask<HackingTerminal> SpawnTerminal(Vector3 position, Transform warZoneTransform);
    UniTask Initialize(AssetReference prefabReference);
  }
}