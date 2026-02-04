using System;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Health
{
  public class Health : MonoBehaviour
  {
    public IReadOnlyReactiveProperty<int> CurrentHealth => _currentHealth;
    public IObservable<UniRx.Unit> Die => _die;
    public IObservable<float> OnDamageTaken => _onDamageTaken;
    
    public int MaxHealth = 100;

    public bool IsAlive => CurrentHealth.Value > 0;

    private ReactiveProperty<int> _currentHealth;
    private readonly Subject<float> _onDamageTaken = new Subject<float>();
    private void Awake()
    {
      _currentHealth = new ReactiveProperty<int>(MaxHealth);
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
    
    
  }
}