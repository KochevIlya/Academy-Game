using System;
using System.ComponentModel;
using _Project.Scripts.Libs.SerializeInterface;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Unit.Animator;
using UniRx;
using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit.Attacker;
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Scripts.Scenes.Game.Unit.Mover;
using _Project.Scripts.Scenes.Game.Unit.Rotator;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Components.Timer;
using System.Linq;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using JetBrains.Annotations;
using _Project.Sounds;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public class GameUnit : MonoBehaviour, IUnitSaveable
  {
    [Inject] private DiContainer _container;
    public UnitAnimator Animator;
    public Health Health;

    public PatrolPath PatrolPath { get; set; } = null;
    private UnitСharacteristicsType _characteristicsType;
    [Inject] private ISaveLoadService _saveLoadService;
    [Inject] private ISoundService _soundService;
    [field: SerializeField] public HealthView HealthView { get; set; }
    [field: SerializeField] public TimerView TimerView { get; set; }
    [field: SerializeField] public Transform WeaponPoint { get; private set; }
    public WeaponBase Weapon { get; private set; }
    public float SpeedMultiplier { get; set; } = 1f;
    public bool HasWeapon { get; private set; }
    public bool IsUnderControl = false;
    public readonly Subject<GameUnit> OnUnitHacked = new Subject<GameUnit>();
    
    [SerializeField] private InterfaceReference<IUnitMover> _mover;
    [SerializeField] private InterfaceReference<IUnitMover> _botMover;
    [SerializeField] private InterfaceReference<IUnitRotator> _rotator;
    [SerializeField] private InterfaceReference<IUnitAttacker> _attacker;
    [SerializeField] private GrenadeExplosionEffect _explosionPrefab;
    //[SerializeField] private float _moveSpeed = 1.5f;
    private IUnitMover _currentMover;
    private UnitStatsData _stats;
    public UnitStatsData Data => _stats;
    public IAbility Ability { get; private set; }
    
    private readonly CompositeDisposable _lifetimeDisposable = new CompositeDisposable();
    
    public GrenadeExplosionEffect SelfDestructionPrefab => _explosionPrefab;
    public IInputControls InputControls { get; private set; }
    [SerializeField] private float _timeToSelfDestroy = 5f;
    [SerializeField] private float _explosionRadius = 3f;
    [SerializeField] private int _explosionDamage = 500;
    
    [Header("Self Destroy VFX (to disable, leave nothing in prefab)")]
    [SerializeField] [CanBeNull] private GameObject VFXPrefab;
    [SerializeField] private Vector3 vfxOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float vfxScaleModifier = 1;
    
    
    private bool _isExploded = false;
    public string Id { get; private set; }
    public void SetId(string id)
    {
      Id = id;
    }
    public float getTimeToSelfDestroy() {  return _timeToSelfDestroy; }
    public UnitStatsData GetStats() => _stats;
    
    private bool isDying = false;
    private void Start()
    { 
      
      _saveLoadService.RegisterUnit(this);
      
      if (HealthView != null)
        HealthView.Initialize(this);
      
      Health.Die.Where(_ => !isDying).Subscribe(_ =>
      {
        isDying = true;
        Weapon = null;
        Destroy(GetComponent<HackableComponent>());
        UpdateControls(new DummyInputControls(InputControls.MousePosition));
        Animator.Die();
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
          collider.enabled = false;
        }
        Destroy(gameObject, 10f);
        
      }).AddTo(this);
      if (TimerView != null)
        TimerView.Initialize(this);
      
      Health.Die.Subscribe(_ =>
      {
        if(IsUnderControl)
          _soundService.Play(Audio.AudioType.Death);
      }).AddTo(this);
    }
    
    public void SetAbility(IAbility ability) 
    {
      Ability = ability;
    }
    private void OnDestroy()
    {
      _saveLoadService.UnregisterUnit(this);
      _lifetimeDisposable.Clear();
    }

    public void UpdateControls(IInputControls inputControls)
    {
      
      if (_currentMover != null) _currentMover.ResetMovement(this);
      _lifetimeDisposable.Clear();

      InputControls = inputControls;

      if (inputControls is UserInputControls)
      {
        _currentMover = _mover.Value;
      }
      else
      {
        _currentMover = _botMover.Value;
      }
      
      SubscribeMovement(inputControls.GetMovementSpeed(_stats));
      SubscribeShoot();
      SubscribeAbility();
      SubscribeRotate();
    }

    public void UpdateWeapon(WeaponBase weapon)
    {
      if (Weapon != null)
        Weapon.Remove();

      HasWeapon = weapon != null;
      Weapon = weapon;
    }

    public void UpdateStats(UnitStatsData unitStats)
    {
      _stats = unitStats;
      Health.UpdateMaxHealth(_stats.maxHealth);
    }

    private void SubscribeRotate()
    {
      Observable.EveryUpdate()
        .Select(_ => InputControls.MousePosition)
        .TakeUntilDestroy(this)
        .Where(_ => InputControls is not DummyInputControls)
        .Subscribe(mousePos => _rotator.Value.Rotate(this, mousePos, Time.deltaTime))
        .AddTo(_lifetimeDisposable);
    }
    
    private void SubscribeMovement(float baseSpeed)
    {
      InputControls.OnMovement
        .Subscribe(delta =>
          {
            float currentSpeed = baseSpeed * SpeedMultiplier;
            _currentMover.Move(this, delta, Time.deltaTime,  currentSpeed);
          })
          .AddTo(_lifetimeDisposable);
    }

    private void SubscribeShoot()
    {
      InputControls.OnShoot
        .Subscribe(_ =>
        {
          _attacker.Value.Attack(this, InputControls.MousePosition);
          
        })
        .AddTo(_lifetimeDisposable);
      
      Animator.OnShootCast
        .Subscribe(_ => _attacker.Value.OnShootCast(this))
        .AddTo(_lifetimeDisposable);
    }

    private void SubscribeAbility()
    {
      InputControls.OnAbilityUse
        .Subscribe(_ => _attacker.Value.AbilityUse(this))
        .AddTo(_lifetimeDisposable);
    }
    
    private void ResetMovement() => _currentMover.ResetMovement(this);
    
    public void DisableControl()
    {
      UpdateControls(new DummyInputControls(InputControls.MousePosition));
      IsUnderControl = false;
      Debug.Log($"[{name}] Управление переведено на Dummy.");
    }
    
    public void UpdateStats(UnitStatsData unitStats, UnitСharacteristicsType type)
    {
      _stats = unitStats;
      _characteristicsType = type;
      Health.UpdateMaxHealth(_stats.maxHealth);
    }
    
    public EnemySaveData GetSaveData()
    {
      if (Health.CurrentHealth.Value <= 0) return null;
      return new EnemySaveData
      {
        Id = this.Id,
        CharacteristicsType = _characteristicsType,
        Position = transform.position,
        CurrentHealth = Health.CurrentHealth.Value,
        customPath = PatrolPathSaveHelper.GetSaveData(PatrolPath)
        
      };
    }

    public void LoadFromData(EnemySaveData data)
    {
      transform.position = data.Position;
    
      Health.SetHealth(data.CurrentHealth);
    }

    public void SelfDestroy()
    {
      Vector3 explosionOrigin = transform.position;
      var prefabFromUnit = SelfDestructionPrefab;
        
      if (prefabFromUnit != null)
      {
        var effect = _container.InstantiatePrefabForComponent<GrenadeExplosionEffect>(
          prefabFromUnit, explosionOrigin, Quaternion.identity, null);
        effect.Initialize(_explosionRadius, 0.5f);
        _soundService.Play(Audio.AudioType.Explosion);
        
        if (IsUnderControl)
        {
          // _soundService.Play(Audio.AudioType.Damage);
        }
      }
        
      Collider[] hitColliders = Physics.OverlapSphere(explosionOrigin, _explosionRadius);
    
      foreach (var hitCollider in hitColliders)
      {
        if (!hitCollider.CompareTag("HitBox")) continue;

        var health = hitCollider.GetComponentInParent<Health>();
        if (health != null)
        {
          health.TakeDamage(_explosionDamage);
        }
      }
      
      if (VFXPrefab is not null)
      {
        var vfxObj = Instantiate(VFXPrefab, gameObject.transform.position + vfxOffset, Quaternion.identity);
        vfxObj.transform.localScale *= vfxScaleModifier;
      }
      
      Debug.Log($"[SelfDestroy] Unit exploded! Damage: {_explosionDamage}, Targets found: {hitColliders.Length}");
      Health.TakeDamage(Int32.MaxValue);
    }
    
    public void DestroyEntity()
    {
      Destroy(gameObject);
    }
    
  }
}