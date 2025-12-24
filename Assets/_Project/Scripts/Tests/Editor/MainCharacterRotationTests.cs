using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Rotator;
using _Project.Scripts.Scenes.Game.Unit.Animator;
using NUnit.Framework;
using UnityEngine;
using FluentAssertions;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Tests.Editor
{
    public class MainCharacterRotationTests
    {
        private MainCharacterRotator _rotator;
        private FakeInputHelper _fakeInputHelper;
        private GameUnit _gameUnit;
        private GameObject _rotatorGo;
        private GameObject _unitGo;
        
        private const float AcceptableRotationError = 45f;
        private const float DefaultDeltaTime = 0.016f;
        private static readonly Vector3 DefaultWorldPosition = new Vector3(5, 0, 5);
        private static readonly Vector3 RotateRightTargetPosition = new Vector3(10, 0, 0);
        private static readonly Vector2 DefaultMouseScreenPos = new Vector2(500, 300);
        

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
            var mouseScreenPos = DefaultMouseScreenPos;
            var worldPosition = DefaultWorldPosition;
            var deltaTime = DefaultDeltaTime;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = worldPosition;

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert
            initialRotation.Should().NotBe(_gameUnit.transform.rotation);
        }

        [Test]
        public void Rotate_WhenScreenToGroundFails_DoesNotRotate()
        {
            // Arrange
            var mouseScreenPos = DefaultMouseScreenPos;
            var deltaTime = DefaultDeltaTime;

            _fakeInputHelper.ShouldReturnTrue = false;

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert
            initialRotation.Should().Be(_gameUnit.transform.rotation);
        }

        [Test]
        public void Rotate_WhenTargetIsAtUnit_DoesNotRotate()
        {
            // Arrange
            var mouseScreenPos = DefaultMouseScreenPos;
            var unitPosition = _gameUnit.transform.position;
            var deltaTime = DefaultDeltaTime;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = unitPosition;

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert
            initialRotation.Should().Be(_gameUnit.transform.rotation);
        }
        

        [Test]
        public void Rotate_WhenTargetIsRight_RotatesRight()
        {
            // Arrange
            var mouseScreenPos = DefaultMouseScreenPos;
            var targetPosition = RotateRightTargetPosition;
            var deltaTime = DefaultDeltaTime;

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

            rotationDifference.Should().BeLessThan(AcceptableRotationError);
        }
        
        [TestCase(0.005f)]
        [TestCase(0.016f)]
        [TestCase(0.033f)]
        [TestCase(0.5f)]
        [TestCase(5f)]
        public void Rotate_WithDifferentDeltaTimes_DoesNotCrash(float deltaTime)
        {
            // Arrange
            var mouseScreenPos = DefaultMouseScreenPos;
            var targetPosition = RotateRightTargetPosition;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = targetPosition;

            var unitTransform = _gameUnit.transform;
            unitTransform.rotation = Quaternion.identity;

            // Act
            Action rotateAction = () => _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);
            

            // Assert
            for (int i = 0; i < 10; i++)
            {
                rotateAction.Should().NotThrow();
            }
            var angleToTarget = Quaternion.FromToRotation(Vector3.forward, targetPosition.normalized);
            var rotationDifference = Quaternion.Angle(unitTransform.rotation, angleToTarget);

            rotationDifference.Should().BeLessThan(AcceptableRotationError);
        }
        
        [TestCase(0f, 0f)]
        [TestCase(1e6f, 1e6f)]
        [TestCase(-1e6f, -1e6f)]
        [TestCase(float.MaxValue / 2, 0f)]
        [TestCase(0f, float.MinValue / 2)]
        public void Rotate_WithExtremeMousePositions_DoesNotThrow(float x, float y)
        {
            // Arrange
            var deltaTime = DefaultDeltaTime;
            var worldPosition = DefaultWorldPosition;
            
            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = worldPosition;
            var mousePos = new Vector2(x, y);

            // Act
            Action act = () => _rotator.Rotate(_gameUnit, mousePos, deltaTime);

            // Assert
            act.Should().NotThrow();
        }
    }
}
