using _Project.Scripts.Infrastructure.Animation;
using _Project.Scripts.Utils;
using UniRx;
using UnityEngine;
namespace _Project.Scripts.Scenes.Game.Unit.Animator
{
  public class UnitAnimator : MonoBehaviour, IAnimationStateReader
  {
    [SerializeField] private UnityEngine.Animator _animator;
    [SerializeField] private float _movementSmoothing = 10f;
    [SerializeField] private float _rotationSmoothing = 10f;

    public readonly ReactiveCommand<UniRx.Unit> OnShootCast = new ReactiveCommand<UniRx.Unit>();

    private Vector2 _lastVelocity = Vector2.zero;
    private Vector2 _lastRotation = Vector2.zero;

    public void Run(Vector2 localDirection, float deltaTime)
    {
      var lerpVector = Vector2.Lerp(_lastVelocity, localDirection, _movementSmoothing * deltaTime);
      _animator.SetFloat(Animations.VelocityX, lerpVector.x);
      _animator.SetFloat(Animations.VelocityY, lerpVector.y);
      _lastVelocity = lerpVector;
    }

    public void Rotate(Vector2 localRotation, float deltaTime)
    {
      if (_lastVelocity.sqrMagnitude > Constants.Epsilon)
        return;

      _lastRotation = Vector2.Lerp(_lastRotation, localRotation, _rotationSmoothing * deltaTime);
      _animator.SetFloat(Animations.VelocityX, _lastRotation.x);
      _animator.SetFloat(Animations.VelocityY, _lastRotation.y);
    }

    public void Shoot() => _animator.SetTrigger(Animations.Shoot);


    public void EnteredState(int stateHash)
    {
      if (stateHash == Animations.Shoot)
        OnShootCast.Execute(UniRx.Unit.Default);
    }
    public void UpdateState(int stateHash) { }
    public void ExitedState(int stateHash) { }
  }
}