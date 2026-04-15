using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;

namespace _Project.Scripts.Scenes.Game.Zones
{
    /// <summary>
    /// Динамический спавнер, который создаёт волны юнитов при уменьшении количества ботов в CombatZone.
    /// Спавн происходит только во время активной фазы боя (после ActivateAggro).
    /// </summary>
    public class InGameSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CombatZone combatZone;

        [Header("Spawn Settings")]
        [SerializeField] private UnitСharacteristicsType unitType;
        [SerializeField] private int triggerThreshold = 2;
        [SerializeField] private int maxActivations = 2;
        [SerializeField] private float delayBetweenWaves = 3f;

        [Inject] private IGameFactory _gameFactory;

        private int _activationsCount = 0;
        private bool _isBattleActive = false;
        private bool _isSpawning = false;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            if (combatZone == null)
            {
                Debug.LogError($"[InGameSpawner] Не указана CombatZone на объекте {gameObject.name}");
                return;
            }

            combatZone.BattleStateChanged
                .Subscribe(OnBattleStateChanged)
                .AddTo(_disposables);
        }

        private void OnBattleStateChanged(bool isBattleActive)
        {
            _isBattleActive = isBattleActive;

            if (isBattleActive)
            {
                Debug.Log($"<color=yellow>[InGameSpawner]</color> Бой начался! Ожидание стабилизации...");
                Observable.Timer(System.TimeSpan.FromSeconds(1f))
                    .Subscribe(_ => TrySpawnOrSubscribe())
                    .AddTo(_disposables);
            }
        }

        private void TrySpawnOrSubscribe()
        {
            if (!_isBattleActive || _isSpawning)
                return;

            if (_activationsCount >= maxActivations)
                return;

            int currentCount = combatZone.GetActiveUnits().Count;

            if (currentCount <= triggerThreshold)
            {
                SpawnWave().Forget();
            }
            else
            {
                SubscribeToUnitDeath();
            }
        }

        private void SubscribeToUnitDeath()
        {
            combatZone.UnitCountChanged
                .Where(count => count <= triggerThreshold)
                .Take(1)
                .Subscribe(_ =>
                {
                    if (_isBattleActive && !_isSpawning && _activationsCount < maxActivations)
                    {
                        SpawnWave().Forget();
                    }
                })
                .AddTo(_disposables);
        }

        private async UniTaskVoid SpawnWave()
        {
            if (_isSpawning) return;
            _isSpawning = true;

            _activationsCount++;

            Debug.Log($"<color=yellow>[InGameSpawner]</color> Волна {_activationsCount}/{maxActivations}. Спавн юнита типа {unitType}");

            GameUnit unit = await _gameFactory.SpawnGameUnit(transform.position, unitType, null);

            if (unit != null)
            {
                combatZone.RegisterUnit(unit);
                combatZone.ActivateAggroOnUnit(unit);
                Debug.Log($"<color=green>[InGameSpawner]</color> Заспавнен юнит {unit.name}, зарегистрирован и переведён в Aggro режим в CombatZone {combatZone.name}");
            }

            Debug.Log($"<color=cyan>[InGameSpawner]</color> Волна {_activationsCount} завершена. Ожидание {delayBetweenWaves}с...");
            await UniTask.Delay(System.TimeSpan.FromSeconds(delayBetweenWaves));
            Debug.Log($"<color=cyan>[InGameSpawner]</color> Задержка завершена. Осталось активаций: {maxActivations - _activationsCount}");

            _isSpawning = false;

            if (_isBattleActive && _activationsCount < maxActivations)
            {
                TrySpawnOrSubscribe();
            }
        }

        private void OnDestroy()
        {
            _disposables.Clear();
        }
    }
}
