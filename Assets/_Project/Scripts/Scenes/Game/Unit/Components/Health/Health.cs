using System;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Health
{
  public class Health : MonoBehaviour
  {
    public IReadOnlyReactiveProperty<int> CurrentHealth => _currentHealth;
    public IObservable<UniRx.Unit> Die => _die;

    public bool IsAlive => CurrentHealth.Value > 0;

    private ReactiveProperty<int> _currentHealth = new();
    private Subject<UniRx.Unit> _die = new();
    
    public void TakeDamage(int amount)
    {
      _currentHealth.Value = Mathf.Max(_currentHealth.Value - amount, 0);
    }
  }
}