using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit._Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GrenadeAbility : MonoBehaviour, IAbility
{
    private IGameFactory _gameFactory;
    private GameUnit _unit;
    private AbilityConfig _abilityConfig;
    
    private GrenadeSettings _settings; 
    
    private float _timer;

    [Inject]
    public void Construct(IGameFactory gameFactory)
    {
        _gameFactory = gameFactory;
    }
    
    public void Initialize(GameUnit unit, AbilityConfig config)
    {
        _unit = unit;
        _abilityConfig = config;
        
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

    private void Update()
    {
        if (_settings != null && _timer > 0)
        {
            _timer -= Time.deltaTime;
        }
    }

    public bool CanUse() => _settings != null && _timer <= 0;

    public void Use(Vector3 targetPosition)
    {
        if (!CanUse()) return;
        
        ThrowGrenade(targetPosition).Forget();
        
        _timer = _settings.cooldown;
    }

    private async UniTaskVoid ThrowGrenade(Vector3 targetPosition)
    {
        if (_settings == null) return;

        var grenade = await _gameFactory.SpawnGrenade(targetPosition);
        
        grenade.Setup(
            targetPosition, 
            _settings.damage, 
            _settings.radius, 
            _settings.fuseTime,
            _settings.speed
        );
    }
}