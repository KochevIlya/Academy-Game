using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GrenadeAbility : MonoBehaviour
{
    private IGameFactory _gameFactory;
    private GameUnit _unit;
    private UnitStatsData _stats;
    
    private float _timer;

    [Inject]
    public void Construct(IGameFactory gameFactory)
    {
        _gameFactory = gameFactory;
    }

    public void Initialize(GameUnit unit, UnitStatsData stats)
    {
        _unit = unit;
        _stats = stats;
        _timer = stats.grenadeThrowCooldown;
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
    }

    public bool CanUse() => _timer <= 0;

    public void Use(Vector3 targetPosition)
    {
        if (!CanUse()) return;
        
        ThrowGrenade(targetPosition).Forget();
        _timer = _stats.grenadeThrowCooldown;
    }

    private async UniTaskVoid ThrowGrenade(Vector3 targetPosition)
    {
        var grenade = await _gameFactory.SpawnGrenade(_unit.transform.position);
        grenade.Setup(targetPosition, _stats.grenadeDamage, _stats.grenadeRadius, _stats.grenadeFuseTime);
    }
}