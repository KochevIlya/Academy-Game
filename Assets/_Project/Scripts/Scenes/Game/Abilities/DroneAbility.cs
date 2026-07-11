using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit._Data;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class DroneAbility : BaseAbility<DroneSettings>
{
    private IGameFactory _gameFactory;
    private IPosessionService _posessionService;
    
    [Inject]    
    public void Construct(IGameFactory gameFactory, IPosessionService posessionService)
    {
        _gameFactory = gameFactory;
        _posessionService = posessionService;
    }

    public override void Use(Vector3 targetPosition)
    {
        Debug.Log($"Drone In Use Drone");
        if (!CanUse()) return;
        _isReady.Value = false;
        _onUsed.OnNext(Unit.Default);
        ThrowDrone(targetPosition).Forget();
        
        _timer = _settings.cooldown;
    }
    
    protected override UniTask UseAbility()
    {
        throw new System.NotImplementedException();
    }

    public override void Initialize(GameUnit unit, AbilityConfig config)
    {
        base.Initialize(unit, config);
        
        var settings = _abilityConfig.GetSettings(BotAbilityType.ThrowDrone) as DroneSettings;
    
        if (settings != null)
        {
            _settings = settings;
            _timer = 0f;
        }
        else
        {
            Debug.LogError($"Drone settings not found in {_abilityConfig.name}");
            enabled = false;
        }
    }
    
    private async UniTaskVoid ThrowDrone(Vector3 targetPosition)
    {
        if (_settings == null) return;
        bool isPlayerCaster = _unit.IsUnderControl;
    
        var drone = await _gameFactory.SpawnDrone(_unit); 
    
        drone.Setup(targetPosition, Settings.speed);
        if (isPlayerCaster)
        {
            _posessionService.Possess(drone);
        }
    }

    public override BotAbilityType GetAbilityType()
    {
        return BotAbilityType.ThrowDrone;
    }
}
