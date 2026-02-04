using System;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Health
{
  public class Health : MonoBehaviour
  {
    public IReadOnlyReactiveProperty<int> CurrentHealth => _currentHealth;
    public IObservable<UniRx.Unit> Die => _die;
    private Subject<int> _onTakeDamage = new();
    public IObservable<int> OnTakeDamage => _onTakeDamage;
    
    public int MaxHealth = 100;

    public bool IsAlive => CurrentHealth.Value > 0;

    private ReactiveProperty<int> _currentHealth;
    private void Awake()
    {
      _currentHealth = new ReactiveProperty<int>(MaxHealth);
    }
    private Subject<UniRx.Unit> _die = new();
    
    public void TakeDamage(int amount)
    {
      _currentHealth.Value = Mathf.Max(_currentHealth.Value - amount, 0);
      _onTakeDamage.OnNext(amount);

      if (_currentHealth.Value <= 0)
      {
        _die.OnNext(UniRx.Unit.Default);
        _die.OnCompleted();
        _onTakeDamage.OnCompleted();
      }
    }
  }
}