using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Utils;
using _Project.Scripts.Utils.Extensions;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit.Rotator
{
    public class MainCharacterRotator : MonoBehaviour, IUnitRotator
    {
        [SerializeField] private float _smoothFactor = 15f;
        [SerializeField] private float _minAngleForAnimation = 1f;

        private IInputHelper _inputHelper;

        [Inject]
        public void Construct(IInputHelper inputHelper)
        {
            _inputHelper = inputHelper;
        }

        public void Rotate(GameUnit gameUnit, Vector2 lookInput, float deltaTime)
        {
            Vector3 worldPosition;

            if (gameUnit.InputControls is UserInputControls)
            {
                if (!_inputHelper.ScreenToGroundPosition(lookInput, gameUnit.transform.position.y, out worldPosition))
                    return;
            }
            else
            {
                worldPosition = new Vector3(lookInput.x, gameUnit.transform.position.y, lookInput.y);
            }

            var targetDirection = (worldPosition - gameUnit.transform.position).SetY(0f);

            if (targetDirection.sqrMagnitude < Constants.Epsilon)
                return;

            RotateUnit(gameUnit, targetDirection, deltaTime);
            AnimateRotation(gameUnit, targetDirection, deltaTime);
        }

        private bool TryGetWorldPosition(GameUnit gameUnit, Vector2 mouseScreenPos, out Vector3 worldPosition)
        {
            return _inputHelper.ScreenToGroundPosition(mouseScreenPos, gameUnit.transform.position.y, out worldPosition);
        }

        private void RotateUnit(GameUnit gameUnit, Vector3 targetDirection, float deltaTime)
        {
            var targetRotation = Quaternion.LookRotation(targetDirection);
            gameUnit.transform.rotation = Quaternion.Slerp(
                gameUnit.transform.rotation,
                targetRotation,
                _smoothFactor * deltaTime
            );
        }

        private void AnimateRotation(GameUnit gameUnit, Vector3 targetDirection, float deltaTime)
        {
            var targetForward = Quaternion.LookRotation(targetDirection) * Vector3.forward;
            var currentForward = gameUnit.transform.forward;
            var angle = Vector3.Angle(targetForward, currentForward);

            if (angle > _minAngleForAnimation)
            {
                RotateAnimator(gameUnit, targetDirection, deltaTime);
            }
            else
            {
                ResetAnimatorRotation(gameUnit, deltaTime);
            }
        }

        private void RotateAnimator(GameUnit gameUnit, Vector3 targetDirection, float deltaTime)
        {
            var localDirection = gameUnit.transform.InverseTransformDirection(targetDirection);
            var rotationX = localDirection.x;
            gameUnit.Animator.Rotate(new Vector2(rotationX, 0f), deltaTime);
        }

        private void ResetAnimatorRotation(GameUnit gameUnit, float deltaTime)
        {
            gameUnit.Animator.Rotate(Vector2.zero, deltaTime);
        }
    }
}
