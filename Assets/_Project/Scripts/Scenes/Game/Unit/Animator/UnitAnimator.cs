using _Project.Scripts.Infrastructure.Animation;
using _Project.Scripts.Utils;
using UniRx;
using UnityEngine;

public class UnitAnimator : MonoBehaviour, IAnimationStateReader
{
  [SerializeField] private Animator _animator;
        
  public readonly ReactiveCommand<Unit> OnShootCast = new();
  
  private Vector2 _lastVelocity = Vector2.zero;

  public void Run(Vector2 velocity, float lerpValue)
  {
    Vector2 lerpVector = Vector2.Lerp(_lastVelocity, velocity, lerpValue);
    _animator.SetFloat(Animations.VelocityX, lerpVector.x);
    _animator.SetFloat(Animations.VelocityY, lerpVector.y);
    _lastVelocity = lerpVector;
  }
  
  public void Shoot() => _animator.SetTrigger(Animations.Shoot);


  public void EnteredState(int stateHash)
  {
    if (stateHash == Animations.Shoot)
      OnShootCast.Execute(Unit.Default);
  }
  public void UpdateState(int stateHash) { }
  public void ExitedState(int stateHash) { }
}
