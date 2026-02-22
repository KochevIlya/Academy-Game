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

namespace _Project.Scripts.Scenes.Game.Unit
{
  public class GameUnit : MonoBehaviour
  {
    public UnitAnimator Animator;
    public Health Health;

    public PatrolPath PatrolPath { get; set; } = null;
    
    [field: SerializeField] public HealthView HealthView { get; set; }
    [field: SerializeField] public Transform WeaponPoint { get; private set; }
    public WeaponBase Weapon { get; private set; }
    public bool HasWeapon { get; private set; }
    public bool IsUnderControl = false;
    public readonly Subject<GameUnit> OnUnitHacked = new Subject<GameUnit>();
    
    [SerializeField] private InterfaceReference<IUnitMover> _mover;
    [SerializeField] private InterfaceReference<IUnitRotator> _rotator;
    [SerializeField] private InterfaceReference<IUnitAttacker> _attacker;
    //[SerializeField] private float _moveSpeed = 1.5f;
    
    private UnitStatsData _stats;

    private readonly CompositeDisposable _lifetimeDisposable = new CompositeDisposable();
    

    public IInputControls InputControls { get; private set; }

    private void Start()
    {
      Health.Die.Subscribe(_ => Destroy(gameObject)).AddTo(this);
    }

    private void OnDestroy()
    {
      _lifetimeDisposable.Clear();
    }

    public void UpdateControls(IInputControls inputControls)
    {
      ResetMovement();
      _lifetimeDisposable.Clear();

      InputControls = inputControls;

      SubscribeMovement();
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
        .Subscribe(mousePos => _rotator.Value.Rotate(this, mousePos, Time.deltaTime))
        .AddTo(_lifetimeDisposable);
    }
    
    private void SubscribeMovement()
    {
      InputControls.OnMovement
        .Subscribe(delta => _mover.Value.Move(this, delta * _stats.speed, Time.deltaTime))
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
    
    private void ResetMovement() => _mover.Value.ResetMovement(this);
    
    public void DisableControl(IInputControls dummyInput)
    {
      UpdateControls(dummyInput);
      IsUnderControl = false;
      Debug.Log($"[{name}] Управление переведено на Dummy.");
    }
    public void SetControlled(bool isControlled)
    {
      IsUnderControl = isControlled;
      if (isControlled)
      {
        OnUnitHacked.OnNext(this);
      }
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
    //               _attacker.Value.Shoot(this, target.transform.position);
    //             })
    //             .AddTo(_lifetimeDisposable);
    //         }
    //       }
    //     }
    //   }
    // }
  }
}