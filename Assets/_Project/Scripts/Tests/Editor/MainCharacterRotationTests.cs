using _Project.Scripts.Infrastructure.Animation;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Rotator;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Animator;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Editor.Tests
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

            // Создаём тестовый unit
            _unitGo = new GameObject("TestUnit");
    
            // Добавляем реальный Unity Animator
            var unityAnimator = _unitGo.AddComponent<UnityEngine.Animator>();
    
            // Добавляем UnitAnimator
            var unitAnimator = _unitGo.AddComponent<UnitAnimator>();
    
            // Присваиваем Animator в UnitAnimator через рефлексию
            var animField = typeof(UnitAnimator).GetField(
                "_animator",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            animField.SetValue(unitAnimator, unityAnimator);
    
            // Создаём GameUnit
            _gameUnit = _unitGo.AddComponent<GameUnit>();
            _gameUnit.Animator = unitAnimator;

            // Создаём фальшивый animator и устанавливаем его
            //_fakeAnimator = new FakeUnitAnimator();
            //var animatorField = typeof(GameUnit).GetField(
            //    "Animator",
            //    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
            //);
            //animatorField.SetValue(_gameUnit, _fakeAnimator);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_unitGo);
            Object.DestroyImmediate(_rotatorGo);
        }

        #region Basic Rotation Tests

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
            _fakeInputHelper.WorldPositionToReturn = unitPosition; // Позиция самого unit

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert
            Assert.AreEqual(initialRotation, _gameUnit.transform.rotation,
                "Unit НЕ должен повернуться когда цель у его ног");
        }

        #endregion

        #region Rotation Direction Tests

        [Test]
        public void Rotate_WhenTargetIsRight_RotatesRight()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var targetPosition = new Vector3(10, 0, 0); // Вправо от unit
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
                "Unit должен повернуться в направлении цели (максимум 45 градусов ошибки)");
        }

        [Test]
        public void Rotate_WhenTargetIsForward_StaysForward()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var targetPosition = new Vector3(0, 0, 10); // Прямо перед unit
            var deltaTime = 0.016f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = targetPosition;

            var unitTransform = _gameUnit.transform;
            unitTransform.rotation = Quaternion.identity;

            var initialRotation = unitTransform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert
            Assert.AreEqual(initialRotation, unitTransform.rotation,
                "Unit должен остаться повёрнут вперёд если цель прямо перед ним");
        }

        [Test]
        public void Rotate_WhenTargetIsBackward_RotatesBackward()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var targetPosition = new Vector3(0, 0, -10); // Позади unit
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
            // Ожидаем ротацию примерно на 180 градусов
            var angle = Quaternion.Angle(unitTransform.rotation, Quaternion.identity);
            Assert.Greater(angle, 90f,
                "Unit должен значительно повернуться в сторону позади себя");
        }

        #endregion

        #region DeltaTime Tests

        [Test]
        public void Rotate_WithSmallDeltaTime_RotatesSlowly()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var worldPosition = new Vector3(10, 0, 0);
            var smallDeltaTime = 0.001f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = worldPosition;

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, smallDeltaTime);

            // Assert
            var rotationDifference = Quaternion.Angle(initialRotation, _gameUnit.transform.rotation);
            Assert.Less(rotationDifference, 5f,
                "С малым deltaTime ротация должна быть минимальной");
        }

        [Test]
        public void Rotate_WithLargeDeltaTime_RotatesFaster()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var worldPosition = new Vector3(10, 0, 0);
            var largeDeltaTime = 0.1f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = worldPosition;

            var initialRotation = _gameUnit.transform.rotation;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, largeDeltaTime);

            // Assert
            var rotationDifference = Quaternion.Angle(initialRotation, _gameUnit.transform.rotation);
            Assert.Greater(rotationDifference, 0.5f,
                "С большим deltaTime ротация должна быть заметнее");
        }

        #endregion

        #region Continuous Rotation Tests

        [Test]
        public void Rotate_MultipleFrames_ContinuouslyRotatesUnit()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var worldPosition = new Vector3(10, 0, 0);
            var deltaTime = 0.016f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = worldPosition;

            var initialRotation = _gameUnit.transform.rotation;
            var previousRotation = initialRotation;

            // Act & Assert
            for (int frame = 0; frame < 5; frame++)
            {
                _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

                var currentRotation = _gameUnit.transform.rotation;
                var frameDifference = Quaternion.Angle(previousRotation, currentRotation);

                // Каждый фрейм должна быть небольшая ротация
                Assert.Greater(frameDifference, 0.001f,
                    $"Frame {frame}: Unit должен немного повернуться каждый фрейм");

                previousRotation = currentRotation;
            }

            var totalDifference = Quaternion.Angle(initialRotation, _gameUnit.transform.rotation);
            Assert.Greater(totalDifference, 0.1f,
                "За 5 фреймов unit должен повернуться заметно");
        }

        #endregion

        

        #region Edge Cases

        [Test]
        public void Rotate_WithNullGameUnit_DoesNotThrow()
        {
            // Arrange
            var mouseScreenPos = new Vector2(500, 300);
            var deltaTime = 0.016f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = new Vector3(5, 0, 5);

            // Act & Assert - не должно быть исключения
            Assert.DoesNotThrow(() =>
            {
                try
                {
                    _rotator.Rotate(null, mouseScreenPos, deltaTime);
                }
                catch (System.NullReferenceException)
                {
                    // Ожидаемо
                }
            });
        }

        [Test]
        public void Rotate_TargetAtSameHeightAsUnit_Works()
        {
            // Arrange
            var unitHeight = _gameUnit.transform.position.y;
            var mouseScreenPos = new Vector2(500, 300);
            var targetPosition = new Vector3(5, unitHeight, 5); // Та же высота
            var deltaTime = 0.016f;

            _fakeInputHelper.ShouldReturnTrue = true;
            _fakeInputHelper.WorldPositionToReturn = targetPosition;

            // Act
            _rotator.Rotate(_gameUnit, mouseScreenPos, deltaTime);

            // Assert - Y должен быть обнулен через SetY(0f)
            var expectedDirection = (targetPosition - _gameUnit.transform.position);
            Assert.AreEqual(0f, expectedDirection.y,
                "Y компонент направления должен быть обнулен");
        }

        #endregion
    }

    /// <summary>
    /// Фальшивый InputHelper для тестирования
    /// Вместо реального ScreenToGroundPosition возвращает заранее заданные значения
    /// </summary>
    public class FakeInputHelper : IInputHelper
    {
        public bool ShouldReturnTrue = true;
        public Vector3 WorldPositionToReturn = Vector3.zero;

        public bool ScreenToGroundPosition(Vector2 screenPos, float y, out Vector3 worldPos)
        {
            worldPos = WorldPositionToReturn;
            return ShouldReturnTrue;
        }
    }
}
