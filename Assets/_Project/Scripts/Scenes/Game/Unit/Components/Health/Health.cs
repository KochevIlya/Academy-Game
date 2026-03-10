using System;
using UniRx;
using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine.Serialization;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Health
{
  public class Health : MonoBehaviour
  {
    //[SerializeField] private UnitStatsData _unitStatsData;
    
    public IReadOnlyReactiveProperty<int> CurrentHealth => _currentHealth;
    public IReadOnlyReactiveProperty<int> MaxHealth => _maxHealth;
    public IObservable<UniRx.Unit> Die => _die;
    public IObservable<float> OnDamageTaken => _onDamageTaken;
    

    public bool IsAlive => CurrentHealth.Value > 0;


    private ReactiveProperty<int> _currentHealth;
    private ReactiveProperty<int> _maxHealth;
    private readonly Subject<float> _onDamageTaken = new Subject<float>();
    private void Awake()
    {
      _maxHealth = new ReactiveProperty<int>(1);
      _currentHealth = new ReactiveProperty<int>(1);
    }
    
    private Subject<UniRx.Unit> _die = new();


    public void TakeDamage(int amount)
    {
      _currentHealth.Value = Mathf.Max(_currentHealth.Value - amount, 0);
      _onDamageTaken.OnNext(amount);
      if (_currentHealth.Value <= 0)
      {
        _die.OnNext(UniRx.Unit.Default);
        _die.OnCompleted();
      }
    }
    
    public void UpdateMaxHealth(int value, bool currentToMax = true)
    {
      _maxHealth.Value = value;
      if (currentToMax) _currentHealth.Value = _maxHealth.Value;
    }

    public void SetHealth(int value)
    {
      _currentHealth.Value = value;
    }
  }
}