using _Project.Scripts.Infrastructure.Animation;
using _Project.Scripts.Utils;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Animator
{
    public class UnitAnimator : MonoBehaviour, IAnimationStateReader
    {
        private const float RotationThreshold = 0.05f;
        private const float MinRotationValue = 0.1f;
        private const float ZeroThreshold = 0.1f;
        
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

        public void Idle()
        {
            _animator.SetFloat(Animations.VelocityX, 0f);
            _animator.SetFloat(Animations.VelocityY, 0f);
        }

        public void Rotate(Vector2 localRotation, float deltaTime)
        {
            if (_lastVelocity.sqrMagnitude > Constants.Epsilon)
                return;

            if (localRotation.sqrMagnitude < Constants.Epsilon)
                SmoothRotationToZero(deltaTime);
            else
                ApplyRotation(localRotation, deltaTime);

            _animator.SetFloat(Animations.VelocityX, _lastRotation.x);
            _animator.SetFloat(Animations.VelocityY, _lastRotation.y);
        }

        private void SmoothRotationToZero(float deltaTime)
        {
            _lastRotation = Vector2.Lerp(_lastRotation, Vector2.zero, _rotationSmoothing * deltaTime);
            if (_lastRotation.sqrMagnitude < RotationThreshold)
                _lastRotation = Vector2.zero;
        }

        private void ApplyRotation(Vector2 localRotation, float deltaTime)
        {
            if (_lastRotation.sqrMagnitude < Constants.Epsilon && Mathf.Abs(localRotation.x) < ZeroThreshold)
            {
                var sign = Mathf.Sign(localRotation.x);
                var minValue = sign * MinRotationValue;
                _lastRotation = Vector2.Lerp(_lastRotation, new Vector2(minValue, 0f), _rotationSmoothing * deltaTime);
            }
            else
            {
                _lastRotation = Vector2.Lerp(_lastRotation, localRotation, _rotationSmoothing * deltaTime);
                if (Mathf.Abs(_lastRotation.x) < RotationThreshold && Mathf.Abs(localRotation.x) < RotationThreshold)
                    _lastRotation = Vector2.zero;
            }
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
