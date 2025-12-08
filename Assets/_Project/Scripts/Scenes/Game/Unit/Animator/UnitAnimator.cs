using _Project.Scripts.Infrastructure.Animation;
using _Project.Scripts.Utils;
using UniRx;
using UnityEngine;

public class UnitAnimator : MonoBehaviour, IAnimationStateReader
{
  [SerializeField] private Animator _animator;
  [SerializeField] private float _velocityThreshold = 0.01f;
  [SerializeField] private float _rotationSmoothing = 10f;
        
  public readonly ReactiveCommand<Unit> OnShootCast = new();
  
  private Vector2 _lastVelocity = Vector2.zero;
  private Vector2 _lastRotation = Vector2.zero;

  public void Run(Vector2 localDirection, float lerpValue)
  {
    Vector2 lerpVector = Vector2.Lerp(_lastVelocity, localDirection, lerpValue);
    _animator.SetFloat(Animations.VelocityX, lerpVector.x);
    _animator.SetFloat(Animations.VelocityY, lerpVector.y);
    _lastVelocity = lerpVector;
  }
  
  public void Rotate(Vector2 localRotation, float deltaTime)
  {
    if (_lastVelocity.sqrMagnitude > _velocityThreshold * _velocityThreshold)
      return;
    
    _lastRotation = Vector2.Lerp(_lastRotation, localRotation, _rotationSmoothing * deltaTime);
    _animator.SetFloat(Animations.VelocityX, _lastRotation.x);
    _animator.SetFloat(Animations.VelocityY, _lastRotation.y);
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
