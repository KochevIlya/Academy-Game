using System;
using UniRx;
using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit._Data;
using JetBrains.Annotations;
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
    
    public float IncomingDamageMultiplier { get; set; } = 1f;
    public bool IsAlive => CurrentHealth.Value > 0;
    
    [Header("Take Damage VFX (to disable, leave nothing in prefab)")]
    [SerializeField] [CanBeNull] private GameObject VFXPrefab;
    [SerializeField] private Vector3 vfxOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float vfxScaleModifier = 1;

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
      int actualAmount = (int)(IncomingDamageMultiplier * amount);
      //Debug.Log($"In Taking Damage IncomingDamageMultiplier: {IncomingDamageMultiplier}, Amount: {amount}, CurrentAmount: {actualAmount}" );
      _currentHealth.Value = Mathf.Max(_currentHealth.Value - actualAmount, 0);
      _onDamageTaken.OnNext(amount);
      
      //Debug.Log($"[HEALTH] In Health Taking Damage VFX {VFXPrefab is not null} ===========================================");
      if (VFXPrefab is not null)
      {
        //Debug.Log("[HEALTH] In Health Taking Damage VFX ===========================================");
        var vfxObj = Instantiate(VFXPrefab, gameObject.transform.position + vfxOffset, Quaternion.identity);
        vfxObj.transform.localScale *= vfxScaleModifier;
      }
      
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
      Debug.Log($"[Health] Current Health: {value}");
      _currentHealth.Value = value;
    }

    public bool IsDead()
    {
      Debug.Log("[Health] In Health IsDead");
      Debug.Log($"[Health] Current Unit Health {_currentHealth.Value}");
      if (_currentHealth.Value <= 0)
        Debug.Log("[Health] Unit dead");
    return _currentHealth.Value <= 0;
    }
  }
}