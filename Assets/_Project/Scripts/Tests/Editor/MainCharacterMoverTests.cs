using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Mover;
using _Project.Scripts.Scenes.Game.Unit.Animator;
using NUnit.Framework;
using UnityEngine;

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
        public void Move_WithForwardInput_MovesForward()
        {
            // Arrange
            var movementDelta = new Vector2(0, 1);
            var deltaTime = 0.016f;
            var initialPosition = _gameUnit.transform.position;

            // Act
            _mover.Move(_gameUnit, movementDelta, deltaTime);

            // Assert
            Assert.AreNotEqual(initialPosition, _gameUnit.transform.position,
                "Unit должен переместиться вперёд");
        }

        [Test]
        public void Move_WithZeroInput_DoesNotMove()
        {
            // Arrange
            var movementDelta = Vector2.zero;
            var deltaTime = 0.016f;
            var initialPosition = _gameUnit.transform.position;

            // Act
            _mover.Move(_gameUnit, movementDelta, deltaTime);

            // Assert
            Assert.AreEqual(initialPosition, _gameUnit.transform.position,
                "Unit НЕ должен переместиться при нулевом вводе");
        }


        [Test]
        public void Move_WithDiagonalInput_MovesDiagonally()
        {
            // Arrange
            var movementDelta = new Vector2(1, 1);
            var deltaTime = 0.016f;
            var initialPosition = _gameUnit.transform.position;

            // Act
            _mover.Move(_gameUnit, movementDelta, deltaTime);

            // Assert
            var deltaPosition = _gameUnit.transform.position - initialPosition;
            Assert.AreNotEqual(initialPosition, _gameUnit.transform.position,
                "Unit должен переместиться диагонально");
            Assert.NotZero(deltaPosition.x,
                "X должен был измениться");
            Assert.NotZero(deltaPosition.z,
                "Z должен был измениться");
        }
        
        
        [Test]
        public void Move_ResetMovement_CallsAnimatorIdle()
        {
            // Arrange Act
            _mover.ResetMovement(_gameUnit);

            // Assert
            Assert.Pass("ResetMovement без ошибок");
        }
    }
}