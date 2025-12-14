using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Rotator;
using _Project.Scripts.Scenes.Game.Unit.Animator;
using NUnit.Framework;
using UnityEngine;

namespace _Project.Scripts.Tests.Editor
{
    public class MainCharacterRotationTests
    {
        private MainCharacterRotator _rotator;
        private FakeInputHelper _fakeInputHelper;
        private GameUnit _gameUnit;
        private GameObject _rotatorGo;
        private GameObject _unitGo;

        [SetUp]
        public void Setup()
        {
            _fakeInputHelper = new FakeInputHelper();
            
            _rotatorGo = new GameObject("Rotator");
            _rotator = _rotatorGo.AddComponent<MainCharacterRotator>();
            
            var field = typeof(MainCharacterRotator).GetField(
                "_inputHelper",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            field.SetValue(_rotator, _fakeInputHelper);


            _unitGo = new GameObject("TestUnit");
            
            var unityAnimator = _unitGo.AddComponent<UnityEngine.Animator>();
            var unitAnimator = _unitGo.AddComponent<UnitAnimator>();
            
            var animField = typeof(UnitAnimator).GetField(
                "_animator",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            animField.SetValue(unitAnimator, unityAnimator);
            
            _gameUnit = _unitGo.AddComponent<GameUnit>();
            _gameUnit.Animator = unitAnimator;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_unitGo);
            Object.DestroyImmediate(_rotatorGo);
        }
        
        
        [Test]
        public void Rotate_WhenValidMousePosition_RotatesUnit()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var worldPosition = new Vector3(5, 0, 5);
            var deltaTime = 0.016f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = worldPosition;

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert
            Assert.AreNotEqual(initialRotation, _gameUnit.transform.rotation,
                "Unit должен повернуться к целевой точке");
        }

        [Test]
        public void Rotate_WhenScreenToGroundFails_DoesNotRotate()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var deltaTime = 0.016f;

            _fakeInputHelper.ShouldReturnTrue = false;

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert
            Assert.AreEqual(initialRotation, _gameUnit.transform.rotation,
                "Unit НЕ должен повернуться если ScreenToGroundPosition вернул false");
        }

        [Test]
        public void Rotate_WhenTargetIsAtUnit_DoesNotRotate()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var unitPosition = _gameUnit.transform.position;
            var deltaTime = 0.016f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = unitPosition;

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert
            Assert.AreEqual(initialRotation, _gameUnit.transform.rotation,
                "Unit НЕ должен повернуться когда цель у его ног");
        }
        

        [Test]
        public void Rotate_WhenTargetIsRight_RotatesRight()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var targetPosition = new Vector3(10, 0, 0);
            var deltaTime = 0.016f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = targetPosition;

            var unitTransform = _gameUnit.transform;
            unitTransform.rotation = Quaternion.identity;

            // Act
            for (int i = 0; i < 10; i++)
            {
                _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);
            }

            // Assert
            var angleToTarget = Quaternion.FromToRotation(Vector3.forward, targetPosition.normalized);
            var rotationDifference = Quaternion.Angle(unitTransform.rotation, angleToTarget);

            Assert.Less(rotationDifference, 45f,
                "Unit должен повернуться в направлении цели");
        }
    }
}
