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
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using System.Linq;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public class GameUnit : MonoBehaviour, IUnitSaveable
  {
    public UnitAnimator Animator;
    public Health Health;

    public PatrolPath PatrolPath { get; set; } = null;
    private UnitСharacteristicsType _characteristicsType;
    [Inject] private ISaveLoadService _saveLoadService;
    [field: SerializeField] public HealthView HealthView { get; set; }
    [field: SerializeField] public Transform WeaponPoint { get; private set; }
    public WeaponBase Weapon { get; private set; }
    public bool HasWeapon { get; private set; }
    public bool IsUnderControl = false;
    public readonly Subject<GameUnit> OnUnitHacked = new Subject<GameUnit>();
    
    [SerializeField] private InterfaceReference<IUnitMover> _mover;
    [SerializeField] private InterfaceReference<IUnitMover> _botMover;
    [SerializeField] private InterfaceReference<IUnitRotator> _rotator;
    [SerializeField] private InterfaceReference<IUnitAttacker> _attacker;
    //[SerializeField] private float _moveSpeed = 1.5f;
    private IUnitMover _currentMover;
    private UnitStatsData _stats;
    public UnitStatsData Data => _stats;
    public IAbility Ability { get; private set; }
    
    private readonly CompositeDisposable _lifetimeDisposable = new CompositeDisposable();
    
    public IInputControls InputControls { get; private set; }
    public string Id { get; private set; }
    public void SetId(string id)
    {
      Id = id;
    }
    public UnitStatsData GetStats() => _stats;
    private void Start()
    { 
      
      _saveLoadService.RegisterUnit(this);
      
      if (HealthView != null)
        HealthView.Initialize(this);
      
      Health.Die.Subscribe(_ => Destroy(gameObject)).AddTo(this);
    }
    public void SetAbility(IAbility ability) 
    {
      Ability = ability;
    }
    private void OnDestroy()
    {
      _saveLoadService.UnregisterUnit(this);
      _lifetimeDisposable.Clear();
      if (PatrolPath != null)
      {
        Destroy(PatrolPath.gameObject);
      }
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
        .Subscribe(mousePos => _rotator.Value.Rotate(this, mousePos, Time.deltaTime))
        .AddTo(_lifetimeDisposable);
    }
    
    private void SubscribeMovement(float speed)
    {
      InputControls.OnMovement
        .Subscribe(delta =>
          {
            _currentMover.Move(this, delta, Time.deltaTime,  speed);
          })
          .AddTo(_lifetimeDisposable);
    }

    private void SubscribeShoot()
    {
      InputControls.OnShoot
        .Subscribe(_ => _attacker.Value.Attack(this, InputControls.MousePosition))
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
      UpdateControls(new DummyInputControls());
      IsUnderControl = false;
      Debug.Log($"[{name}] Управление переведено на Dummy.");
    }
    
    // private void OnTriggerEnter(Collider other)
    // {
    //   if (InputControls is DummyInputControls)
    //   {
    //     if (other.CompareTag("Bullet"))
    //     {
    //       var bullet = other.GetComponent<Bullet>();
    //       if (bullet != null)
    //       {
    //         var target = FindObjectsOfType<GameUnit>()
    //           .FirstOrDefault(unit => unit.IsUnderControl);
    //
    //         if (target != null)
    //         {
    //           Observable.EveryUpdate()
    //             .TakeUntil(target.Health.Die)
    //             .Subscribe(_ =>
    //             {
    //               _attacker.Value.Attack(this, target.transform.position);
    //             })
    //             .AddTo(_lifetimeDisposable);
    //         }
    //       }
    //     }
    //   }
    // }

    public void UpdateStats(UnitStatsData unitStats, UnitСharacteristicsType type)
    {
      _stats = unitStats;
      _characteristicsType = type;
      Health.UpdateMaxHealth(_stats.maxHealth);
    }
    
    public EnemySaveData GetSaveData()
    {
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

    public void DestroyEntity()
    {
      Destroy(gameObject);
    }
  }
}