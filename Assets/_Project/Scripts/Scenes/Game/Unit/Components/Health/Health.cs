using System;
using UniRx;
using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit._Configs;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Health
{
  public class Health : MonoBehaviour
  {
    [SerializeField] private UnitStatsConfig _unitStatsConfig;
    
    public IReadOnlyReactiveProperty<int> CurrentHealth => _currentHealth;
    public IObservable<UniRx.Unit> Die => _die;
    
    public int MaxHealth => (_unitStatsConfig)? _unitStatsConfig.maxHealth : 1;

    public bool IsAlive => CurrentHealth.Value > 0;


    private ReactiveProperty<int> _currentHealth;
    private void Awake()
    {
      _currentHealth = new ReactiveProperty<int>(MaxHealth);
    }
    
    private Subject<UniRx.Unit> _die = new();

    private void Awake()
    {
      _currentHealth = new(MaxHealth);
    }

    public void TakeDamage(int amount)
    {
      _currentHealth.Value = Mathf.Max(_currentHealth.Value - amount, 0);

      if (_currentHealth.Value <= 0)
      {
        _die.OnNext(UniRx.Unit.Default);
        _die.OnCompleted();
      }
    }
  }
}