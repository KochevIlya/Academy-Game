using UniRx;
using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit.Attacker;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public class GameUnit : MonoBehaviour
  {
    [field: SerializeField] public UnitAnimator Animator;

    [SerializeField] private InterfaceReference<IUnitMover> _mover;
    [SerializeField] private InterfaceReference<IUnitRotator> _rotator;
    [SerializeField] private InterfaceReference<IUnitAttacker> _attacker;

    private readonly CompositeDisposable _lifetimeDisposable = new CompositeDisposable();

    public IInputControls InputControls { get; private set; }

    private void OnDestroy()
    {
      _lifetimeDisposable.Clear();
    }

    public void UpdateControls(IInputControls inputControls)
    {
      _lifetimeDisposable.Clear();

      InputControls = inputControls;

      SubscribeMovement();
      SubscribeShoot();
      SubscribeAbility();
      SubscribeRotate();
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
        .Subscribe(delta => _mover.Value.Move(this, delta, Time.deltaTime))
        .AddTo(_lifetimeDisposable);
    }

    private void SubscribeShoot()
    {
      InputControls.OnShoot
        .Subscribe(_ => _attacker.Value.Shoot(this))
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
  }
}