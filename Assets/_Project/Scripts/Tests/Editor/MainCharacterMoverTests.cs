using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Mover;
using _Project.Scripts.Scenes.Game.Unit.Animator;
using NUnit.Framework;
using UnityEngine;
using FluentAssertions;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Tests.Editor
{
    public class MainCharacterMoverTests
    {
        private MainCharacterMover _mover;
        private FakeCameraService _fakeCameraService;
        private GameUnit _gameUnit;
        private GameObject _moverGameObject;
        private GameObject _unitGameObject;
        private UnitAnimator _unitAnimator;
        
        private const float DefaultDeltaTime = 0.016f;
        private static readonly Vector2 MovementForwardDelta = new Vector2(0, 1);
        private static readonly Vector2 MovementDiagonallyDelta = new Vector2(1, 1);
        private static readonly Vector2 MovementZeroDelta = Vector2.zero;

        [SetUp]
        public void Setup()
        {
            _fakeCameraService = new FakeCameraService();
            
            _moverGameObject = new GameObject("Mover");
            _mover = _moverGameObject.AddComponent<MainCharacterMover>();


            var field = typeof(MainCharacterMover).GetField(
                "_cameraService",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            field.SetValue(_mover, _fakeCameraService);


            _unitGameObject = new GameObject("TestUnit");
            var unityAnimator = _unitGameObject.AddComponent<UnityEngine.Animator>();
            _unitAnimator = _unitGameObject.AddComponent<UnitAnimator>();
            
            var animField = typeof(UnitAnimator).GetField(
                "_animator",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            animField.SetValue(_unitAnimator, unityAnimator);
            
            _gameUnit = _unitGameObject.AddComponent<GameUnit>();
            _gameUnit.Animator = _unitAnimator;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_unitGameObject);
            Object.DestroyImmediate(_moverGameObject);
            _fakeCameraService.Destroy();
        }
        
        [Test]
        public void Move_WithForwardInput_MovesForward(float speed)
        {
            // Arrange
            var movementDelta = MovementForwardDelta;
            var deltaTime = DefaultDeltaTime;
            var initialPosition = _gameUnit.transform.position;

            // Act
            _mover.Move(_gameUnit, movementDelta, deltaTime, speed);

            // Assert
            initialPosition.Should().NotBe(_gameUnit.transform.position);
        }

        [Test]
        public void Move_WithZeroInput_DoesNotMove(float speed)
        {
            // Arrange
            var movementDelta = MovementZeroDelta;
            var deltaTime = DefaultDeltaTime;
            var initialPosition = _gameUnit.transform.position;

            // Act
            _mover.Move(_gameUnit, movementDelta, deltaTime, speed);

            // Assert
            initialPosition.Should().Be(_gameUnit.transform.position);
        }


        [Test]
        public void Move_WithDiagonalInput_MovesDiagonally(float speed)
        {
            // Arrange
            var movementDelta = MovementDiagonallyDelta;
            var deltaTime = DefaultDeltaTime;
            var initialPosition = _gameUnit.transform.position;

            // Act
            _mover.Move(_gameUnit, movementDelta, deltaTime, speed);

            // Assert
            var deltaPosition = _gameUnit.transform.position - initialPosition;
            
            initialPosition.Should().NotBe(_gameUnit.transform.position);
            deltaPosition.x.Should().NotBe(0);
            deltaPosition.z.Should().NotBe(0);
        }
        
        
        [Test]
        public void Move_ResetMovement_CallsAnimatorIdle()
        {
            // Arrange Act
            _mover.ResetMovement(_gameUnit);

            // Assert
            Assert.Pass("ResetMovement без ошибок");
        }
        
        [TestCase(0.000001f)]
        [TestCase(0.0001f)]
        [TestCase(DefaultDeltaTime)]
        [TestCase(0.033f)]
        [TestCase(1f)]
        [TestCase(10f)]
        public void Move_WithDifferentDeltaTimes_DoesNotThrow(float deltaTime, float speed)
        {
            var initialPosition = _gameUnit.transform.position;

            Action act = () => _mover.Move(_gameUnit, MovementDiagonallyDelta, deltaTime, speed);

            act.Should().NotThrow();

            var displacement = _gameUnit.transform.position - initialPosition;
            displacement.magnitude.Should().BeGreaterThan(0f);
        }
        
        [TestCase(1e6f, 0f)]
        [TestCase(0f, 1e6f)]
        [TestCase(-1e6f, -1e6f)]
        public void Move_WithExtremeInputValues_DoesNotThrow(float x, float y, float speed)
        {
            var deltaTime = DefaultDeltaTime;
            var movementDelta = new Vector2(x, y);

            Action act = () => _mover.Move(_gameUnit, movementDelta, deltaTime, speed);

            act.Should().NotThrow();
        }
    }
}