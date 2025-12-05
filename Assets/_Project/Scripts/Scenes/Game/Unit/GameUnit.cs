using _Project.Scripts.Scenes.Game.Unit.Attacker;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public class GameUnit : MonoBehaviour
  {
    private IInputControls _inputControls;
    private IUnitMover _mover;
    private IUnitRotator _rotator;
    
    private readonly CompositeDisposable _lifetimeDisposable = new CompositeDisposable();
    private IUnitAttacker _attacker;

    private void OnDestroy()
    {
      _lifetimeDisposable.Clear();
    }

    public void UpdateControls(IInputControls inputControls)
    {
      UpdateControls(inputControls, _mover, _rotator, _attacker);
    }
    
    public void UpdateControls(IInputControls inputControls, IUnitMover mover, IUnitRotator rotator, IUnitAttacker unitAttacker)
    {
      _lifetimeDisposable.Clear();
      
      _inputControls = inputControls;
      _mover = mover;
      _rotator = rotator;
      _attacker = unitAttacker;

      _inputControls.OnMovement
        .Subscribe(_mover.Move)
        .AddTo(_lifetimeDisposable);
      
      _inputControls.OnRotation
        .Subscribe(_rotator.Rotate)
        .AddTo(_lifetimeDisposable);
      
      _inputControls.OnShoot
        .Subscribe(_attacker.Shoot)
        .AddTo(_lifetimeDisposable);

      _inputControls.OnAbilityUse
        .Subscribe(_ => _attacker.AbilityUse())
        .AddTo(_lifetimeDisposable);
    }
  }
}
