using UniRx;
using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit.Attacker;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public class GameUnit : MonoBehaviour
  {
    [field: SerializeField] public UnitAnimator Animator;
    
    private IUnitMover _mover;
    private IUnitAttacker _attacker;
    
    private readonly CompositeDisposable _lifetimeDisposable = new CompositeDisposable();

    public  IInputControls InputControls { get; private set; }
    
    private void OnDestroy()
    {
      _lifetimeDisposable.Clear();
    }

    public void UpdateControls(IInputControls inputControls)
    {
      UpdateControls(inputControls, _mover, _attacker);
    }
    
    public void UpdateControls(IInputControls inputControls, IUnitMover mover, IUnitAttacker unitAttacker)
    {
      _lifetimeDisposable.Clear();
      
      InputControls = inputControls;
      _mover = mover;
      _attacker = unitAttacker;

      SubscribeMovement();
      SubscribeShoot();
      SubscribeAbility();
    }

    private void SubscribeMovement()
    {
      InputControls.OnMovement
        .Subscribe(delta => _mover.Move(this, delta))
        .AddTo(_lifetimeDisposable);
    }

    private void SubscribeShoot()
    {
      InputControls.OnShoot
        .Subscribe(_ => _attacker.Shoot(this))
        .AddTo(_lifetimeDisposable);

      Animator.OnShootCast
        .Subscribe(_ => _attacker.OnShootCast(this))
        .AddTo(_lifetimeDisposable);
    }
    
    private void SubscribeAbility()
    {
      InputControls.OnAbilityUse
        .Subscribe(_ => _attacker.AbilityUse(this))
        .AddTo(_lifetimeDisposable);
    }
  }
}
