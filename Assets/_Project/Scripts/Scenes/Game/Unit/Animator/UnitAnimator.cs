using _Project.Scripts.Infrastructure.Animation;
using _Project.Scripts.Utils;
using UniRx;
using UnityEngine;

public class UnitAnimator : MonoBehaviour, IAnimationStateReader
{
  [SerializeField] private Animator _animator;
        
  public readonly ReactiveCommand<Unit> OnShootCast = new();

  public void Run(float speed) => _animator.SetFloat(Animations.Velocity, speed);
  public void Shoot() => _animator.SetTrigger(Animations.Shoot);


  public void EnteredState(int stateHash)
  {
    if (stateHash == Animations.Shoot)
      OnShootCast.Execute(Unit.Default);
  }
  public void UpdateState(int stateHash) { }
  public void ExitedState(int stateHash) { }
}
