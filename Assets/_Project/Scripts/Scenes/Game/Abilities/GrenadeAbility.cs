using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit._Data;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class GrenadeAbility : BaseAbility<GrenadeSettings>
{
    private IGameFactory _gameFactory;
    
    protected readonly ReactiveProperty<bool> _isReady = new ReactiveProperty<bool>(false);

    [Inject]    
    public void Construct(IGameFactory gameFactory)
    {
        _gameFactory = gameFactory;
    }
    
    public override void Initialize(GameUnit unit, AbilityConfig config)
    {
        base.Initialize(unit, config);
        
        var settings = _abilityConfig.GetSettings(BotAbilityType.ThrowGrenade) as GrenadeSettings;
    
        if (settings != null)
        {
            _settings = settings;
            _timer = 0f;
        }
        else
        {
            Debug.LogError($"Grenade settings not found in {_abilityConfig.name}");
            enabled = false;
        }
    }
    

    public override void Use(Vector3 targetPosition)
    {
        Debug.Log($"Grenade In Use Grenade");
        if (!CanUse()) return;
        _isReady.Value = false;
        _onUsed.OnNext(Unit.Default);
        ThrowGrenade(targetPosition).Forget();
        
        _timer = _settings.cooldown;
    }

    protected override UniTask UseAbility()
    {
        throw new System.NotImplementedException();
    }

    private async UniTaskVoid ThrowGrenade(Vector3 targetPosition)
    {
        if (_settings == null) return;
        
        var grenade = await _gameFactory.SpawnGrenade(targetPosition);
        
        grenade.Setup(
            targetPosition, 
            Settings.damage, 
            Settings.radius, 
            Settings.fuseTime,
            Settings.speed
        );
    }
    public override BotAbilityType GetAbilityType()
    {
        return BotAbilityType.ThrowGrenade;
    }
}