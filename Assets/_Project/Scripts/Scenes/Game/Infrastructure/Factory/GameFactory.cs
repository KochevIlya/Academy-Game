using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.AssetProvider;
using _Project.Scripts.Infrastructure.StaticData;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using _Project.Scripts.Libs.Pool;

namespace _Project.Scripts.Scenes.Game.Infrastructure.Factory
{

  public class GameFactory : IGameFactory
  {
    private readonly IStaticDataService _staticData;
    private readonly DiContainer _diContainer;
    private readonly UserInputControls _userInputControls;
    private readonly DummyInputControls _dummyInputControls;
    private readonly IAssetProvider _assetProvider;
    
    private ObjectPool<Bullet> _bulletPool;
    
    public GameFactory(IStaticDataService staticData, DiContainer diContainer, 
      UserInputControls userInputControls, DummyInputControls dummyInputControls, 
      IAssetProvider assetProvider)
    {
      _staticData = staticData;
      _diContainer = diContainer;
      _userInputControls = userInputControls;
      _dummyInputControls = dummyInputControls;
      _assetProvider = assetProvider;
    }
    
    public async UniTask<GameUnit> SpawnCharacter(Vector3 position, WeaponType weapon)
    {
      var prefab = await _assetProvider.LoadFromAddressable<GameObject>(_staticData.UnitsConfig.Character);

      GameUnit character = _diContainer
        .InstantiatePrefabForComponent<GameUnit>(prefab, 
          position, Quaternion.identity, null);
      
      character.UpdateWeapon(await SpawnWeapon(weapon, character));
      character.UpdateControls(_userInputControls);
      
      return character;
    }

    public async UniTask<GameUnit> SpawnBot(Vector3 position)
    {
      var prefab = await _assetProvider.LoadFromAddressable<GameObject>(_staticData.UnitsConfig.Bot);
      
      GameUnit bot = _diContainer
        .InstantiatePrefabForComponent<GameUnit>(prefab, 
          position, Quaternion.identity, null);
      
      bot.UpdateControls(_dummyInputControls);
      return bot;
    }
    
    public async UniTask<WeaponBase> SpawnWeapon(WeaponType weaponType, GameUnit unit)
    {
      var weaponData = _staticData.WeaponsConfig.Weapons[weaponType];
      var prefab = await _assetProvider.LoadFromAddressable<GameObject>(weaponData.Prefab);
      
      var weapon = _diContainer
        .InstantiatePrefabForComponent<WeaponBase>(prefab,
          unit.WeaponPoint.position, Quaternion.identity, unit.WeaponPoint);
      
      weapon.Setup(weaponData, unit);
      return weapon;
    }
    
    public async UniTask<Bullet> SpawnBullet(AssetReference prefabRefence, Transform spawnPoint)
    {
      if (_bulletPool == null)
      {
        await Initialize(prefabRefence);
      }
      var bullet = _bulletPool.Spawn();
      bullet.transform.position = spawnPoint.position;
      bullet.transform.rotation = spawnPoint.rotation;
      bullet.gameObject.SetActive(true);
    
      return bullet;
    }
    
    public async UniTask Initialize(AssetReference prefabRefence)
    {
      var bulletPrefab = await _assetProvider.LoadFromAddressable<GameObject>(prefabRefence);
    
      _bulletPool = new ObjectPool<Bullet>(() =>
      {
        var bullet = _diContainer
          .InstantiatePrefabForComponent<Bullet>(bulletPrefab,
            Vector3.zero, Quaternion.identity, null);
        bullet.OnCreated(_bulletPool);
        bullet.gameObject.SetActive(false);
        return bullet;
      }, 0);
    }
  }
}