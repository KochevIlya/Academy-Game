using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Utils;
using _Project.Scripts.Utils.Extensions;
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

        public void Rotate(GameUnit gameUnit, Vector2 mouseScreenPos, float deltaTime)
        {
            Vector3 worldPosition;
            if (gameUnit.InputControls is AggroInputControls aggro)
            {
                worldPosition = aggro.TargetPosition; 
            }
            else
            {
                if (!TryGetWorldPosition(gameUnit, mouseScreenPos, out worldPosition))
                    return;
            }
            
            Vector3 targetDirection;
            if (gameUnit.HasWeapon)
            {
                targetDirection = (worldPosition - gameUnit.Weapon.SpawnPoint.position).SetY(0f);
            }
            else
            {
                targetDirection = (worldPosition - gameUnit.transform.position).SetY(0f);
            }
            if (targetDirection.sqrMagnitude < Constants.Epsilon)
                return;

            RotateUnit(gameUnit, targetDirection.normalized, deltaTime);
            AnimateRotation(gameUnit, targetDirection.normalized, deltaTime);
        }

        private bool TryGetWorldPosition(GameUnit gameUnit, Vector2 mouseScreenPos, out Vector3 worldPosition)
        {
            float targetHeight = (gameUnit.HasWeapon) 
                ? gameUnit.Weapon.SpawnPoint.position.y 
                : 1.2f; 

            return _inputHelper.ScreenToGroundPosition(mouseScreenPos, targetHeight, out worldPosition);
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
