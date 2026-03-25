using System.Collections.Generic;
using _Project.Scripts.Infrastructure.AssetProvider;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Infrastructure.StaticData;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using _Project.Scripts.Libs.Pool;
using _Project.Scripts.Scenes.Game.Abilities;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls.Variants;

namespace _Project.Scripts.Scenes.Game.Infrastructure.Factory
{

  public class GameFactory : IGameFactory
  {
    private readonly IStaticDataService _staticData;
    private readonly DiContainer _diContainer;
    private readonly UserInputControls _userInputControls;
    private readonly IAssetProvider _assetProvider;
    private readonly Dictionary<string, Libs.Pool.ObjectPool<Bullet>> _bulletPools = new();
    private IInputHelper _inputHelper;
    private ICameraService _cameraService { get; set; }
    private readonly ICursorService _cursorService;
    public GameFactory(
      IStaticDataService staticData,
      DiContainer diContainer, 
      UserInputControls userInputControls,
      IAssetProvider assetProvider,
      ICameraService cameraService,
      ICursorService cursorService
      )
    {
      
      _staticData = staticData;
      _diContainer = diContainer;
      _userInputControls = userInputControls;
      _assetProvider = assetProvider;
      _cameraService = cameraService;
      _cursorService = cursorService;
    }
    public async UniTask<GameUnit> SpawnGameUnit(Vector3 position,
      UnitСharacteristicsType unitСharacteristicsType,
      PatrolPath path)
    {
      var unitData = _staticData.UnitStatsConfig.Units[unitСharacteristicsType];
      
      var prefabReference = _staticData.UnitsConfig.
        GetPrefabForBehaviour(unitData.behaviourType);
      
      var prefab = await _assetProvider.
        LoadFromAddressable<GameObject>(prefabReference);
      
      
      GameUnit bot = _diContainer
        .InstantiatePrefabForComponent<GameUnit>(prefab, 
          position, Quaternion.identity, null);
      
      bot.SetId(System.Guid.NewGuid().ToString());
      
      bot.UpdateStats(unitData, unitСharacteristicsType);
      
      if (unitData.behaviourType != UnitBehaviourType.Character)
      {
        
        if (unitData.abilityType == BotAbilityType.ThrowGrenade)
          AddAbility<GrenadeAbility>(bot, unitData.ability);
        
        else if (unitData.abilityType == BotAbilityType.Dash)
          AddAbility<DashAbility>(bot, unitData.ability);
        
        else if(unitData.abilityType == BotAbilityType.Shield)
          AddAbility<ShieldAbility>(bot, unitData.ability);
        
        bot.UpdateWeapon(await SpawnWeapon(unitData.weaponType, bot));
        bot.AddComponent<HackableComponent>();
        bot.PatrolPath = path;
        bot.UpdateControls(new PatrolInputControls(bot));
      }
      else
      {
        
        CreateCrosshair().Forget();
        _cameraService.SetTarget(bot);
        var hacker = bot.gameObject.AddComponent<PlayerHacker>();
        _diContainer.Inject(hacker);
        bot.UpdateControls(_userInputControls);
        
        
        
      }
      return bot;
    }
    
    private void AddAbility<T>(GameUnit bot, AbilityConfig config) where T : MonoBehaviour, IAbility
    {
      var abilityComponent = bot.gameObject.AddComponent<T>();
      _diContainer.Inject(abilityComponent);
      abilityComponent.Initialize(bot, config);
      bot.SetAbility(abilityComponent);
    }
    
    public async UniTask<Grenade> SpawnGrenade(Vector3 position)
    {
      var prefabReference = _staticData.UnitsConfig.Grenade;
      
      var prefab = await _assetProvider.
        LoadFromAddressable<GameObject>(prefabReference);
    
      return _diContainer.InstantiatePrefabForComponent<Grenade>
        (prefab, position, Quaternion.identity, null);
    }

    public async UniTask<GameUnit> RestoreGameUnit(EnemySaveData data)
    {
      PatrolPath path = PatrolPathSaveHelper.RestorePath(data.customPath, data.Id);
      GameUnit unit = await SpawnGameUnit(data.Position, data.CharacteristicsType, path);
      unit.SetId(data.Id);
      unit.LoadFromData(data);
      
      return unit;
    }


    public async UniTask<HackingTerminal> SpawnTerminal(Vector3 position, Transform warZoneTransform, string id)
    {
      var prefab = await _assetProvider.LoadFromAddressable<GameObject>(_staticData.TerminalConfig.Prefab);
      GameObject terminalObject =
        _diContainer.InstantiatePrefab(prefab, position, Quaternion.identity, null);
      HackingTerminal terminal = terminalObject.GetComponentInChildren<HackingTerminal>();
      
      
      terminal.WarZoneTransform = warZoneTransform; 
      SaveTrigger trigger = terminalObject.GetComponentInChildren<SaveTrigger>();
      trigger.SetId(id);
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
    
    public async UniTask<Bullet> SpawnBullet(AssetReference prefabReference, Transform spawnPoint)
    {
      string key = prefabReference.AssetGUID;
      if (!_bulletPools.ContainsKey(key))
      {
        await Initialize(prefabReference);
      }
      var bullet = _bulletPools[key].Spawn();
      if (spawnPoint != null)
      {
        bullet.transform.position = spawnPoint.position;
        bullet.transform.rotation = spawnPoint.rotation;
      }
    
      bullet.gameObject.SetActive(true);
      return bullet;
    }

    public async UniTask Initialize(AssetReference prefabReference)
    {
      if (prefabReference == null || !prefabReference.RuntimeKeyIsValid())
        return;

      string key = prefabReference.AssetGUID;

      if (_bulletPools.ContainsKey(key)) return;

      var bulletPrefab = await _assetProvider.LoadFromAddressable<GameObject>(prefabReference);

      if (_bulletPools.ContainsKey(key)) return;

      ObjectPoolSpawnable<Bullet> pool = null;

      pool = new ObjectPoolSpawnable<Bullet>(() =>
      {
        var bullet = _diContainer
          .InstantiatePrefabForComponent<Bullet>(bulletPrefab,
            Vector3.zero, Quaternion.identity, null);
        
        bullet.OnCreated(pool); 
        bullet.gameObject.SetActive(false);
        return bullet;
      }, 20);

      _bulletPools.TryAdd(key, pool);

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