using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.AssetProvider;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Infrastructure.StaticData;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using _Project.Scripts.Libs.Pool;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using UnityEngine.Rendering;

namespace _Project.Scripts.Scenes.Game.Infrastructure.Factory
{

  public class GameFactory : IGameFactory
  {
    private readonly IStaticDataService _staticData;
    private readonly DiContainer _diContainer;
    private readonly UserInputControls _userInputControls;
    private readonly DummyInputControls _dummyInputControls;
    private readonly IAssetProvider _assetProvider;
    private bool _isBulletPoolReady;
    private Libs.Pool.ObjectPool<Bullet> _bulletPool;
    private IInputHelper _inputHelper;
    private ICameraService _cameraService { get; set; }
    private ICursorService _cursorService;
    public GameFactory(IStaticDataService staticData, DiContainer diContainer, 
      UserInputControls userInputControls, DummyInputControls dummyInputControls, 
      IAssetProvider assetProvider,
      ICameraService cameraService,
      ICursorService cursorService)
    {
      
      _staticData = staticData;
      _diContainer = diContainer;
      _userInputControls = userInputControls;
      _dummyInputControls = dummyInputControls;
      _assetProvider = assetProvider;
      _cameraService = cameraService;
      _cursorService = cursorService;
    }
    
    public async UniTask<GameUnit> SpawnCharacter(Vector3 position, WeaponType weapon)
    {
      CreateCrosshair().Forget();
      var prefab = await _assetProvider.LoadFromAddressable<GameObject>(_staticData.UnitsConfig.Character);
      GameUnit character = _diContainer
        .InstantiatePrefabForComponent<GameUnit>(prefab, 
          position, Quaternion.identity, null);
      
      _cameraService.SetTarget(character);
      character.HealthView.Initialize(character);
      var hacker = character.gameObject.AddComponent<PlayerHacker>();
      _diContainer.Inject(hacker);
      // character.UpdateWeapon(await SpawnWeapon(weapon, character));
      character.UpdateControls(_userInputControls);
      character.UpdateStats(_staticData.UnitStatsConfig.Units[UnitСharacteristicsType.MainCharacter]);
      
      return character;
    }

    public async UniTask<GameUnit> SpawnBot(Vector3 position, WeaponType weapon, UnitСharacteristicsType unitСharacteristicsType,
      PatrolPath path)
    {
      var prefab = await _assetProvider.LoadFromAddressable<GameObject>(_staticData.UnitsConfig.Bot);
      var unitData = _staticData.UnitStatsConfig.Units[unitСharacteristicsType];
      
      GameUnit bot = _diContainer
        .InstantiatePrefabForComponent<GameUnit>(prefab, 
          position, Quaternion.identity, null);
      
      bot.HealthView.Initialize(bot);
      bot.AddComponent<HackableComponent>();
      bot.UpdateWeapon(await SpawnWeapon(weapon, bot));
      bot.UpdateStats(unitData);
      bot.PatrolPath = path;
      bot.UpdateControls(new PatrolInputControls(bot));
      return bot;
    }
    
    public async UniTask<HackingTerminal> SpawnTerminal(Vector3 position, Transform warZoneTransform)
    {
      var prefab = await _assetProvider.LoadFromAddressable<GameObject>(_staticData.TerminalConfig.Prefab);
      GameObject terminalObject =
        _diContainer.InstantiatePrefab(prefab, position, Quaternion.identity, null);
      HackingTerminal terminal = terminalObject.GetComponentInChildren<HackingTerminal>();
      terminal.WarZoneTransform = warZoneTransform; 
      return terminal;
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
      var bullet = _bulletPool.Spawn();
      bullet.transform.position = spawnPoint.position;
      bullet.transform.rotation = spawnPoint.rotation;
      bullet.gameObject.SetActive(true);
    
      return bullet;
    }

    public async UniTask Initialize(AssetReference prefabReference)
    {
      var bulletPrefab = await _assetProvider.LoadFromAddressable<GameObject>(prefabReference);

      _bulletPool = new ObjectPoolSpawnable<Bullet>(() =>
      {
        var bullet = _diContainer
          .InstantiatePrefabForComponent<Bullet>(bulletPrefab,
            Vector3.zero, Quaternion.identity, null);
        bullet.OnCreated(_bulletPool);
        bullet.gameObject.SetActive(false);
        return bullet;
      }, 20);
      _isBulletPoolReady = true;
    }

    public async UniTask CreateCrosshair()
    {
      var prefabReference = _staticData.UnitsConfig.Crosshair; 
      var prefab = await _assetProvider.LoadFromAddressable<GameObject>(prefabReference);
      GameObject crosshairInstance = _diContainer
        .InstantiatePrefab(prefab, Vector3.zero, Quaternion.identity, null);
      if (crosshairInstance.TryGetComponent(out CursorController controller))
      {
        _cursorService.Register(controller);
        controller.Initialize(_userInputControls);
      }
      else
      {
        Debug.LogError("На префабе прицела нет скрипта CrosshairController!");
      }
    } 
  }
  
}