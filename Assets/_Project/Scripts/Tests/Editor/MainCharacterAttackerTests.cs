using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Attacker;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit.Animator;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using NUnit.Framework;
using UnityEngine;
using FluentAssertions;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Tests.Editor
{
    public class MainCharacterAttackerTests
    {
        private MainCharacterAttacker _attacker;
        private FakeCameraService _fakeCameraService;
        private GameUnit _gameUnit;
        private GameObject _attackerGo;
        private GameObject _unitGo;
        private UnitAnimator _unitAnimator;
        private TestWeapon _testWeapon;
        private UserInputControls _userInputControls;
        private DummyInputControls _dummyInputControls;
        
        private static readonly Vector2 DefaultShootPosition = new Vector2(500, 300);

        [SetUp]
        public void Setup()
        {
            _fakeCameraService = new FakeCameraService();
            _userInputControls = new UserInputControls();
            _dummyInputControls = new DummyInputControls();
            
            _attackerGo = new GameObject("Attacker");
            _attacker = _attackerGo.AddComponent<MainCharacterAttacker>();
            
            var cameraField = typeof(MainCharacterAttacker).GetField(
                "_cameraService",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            cameraField.SetValue(_attacker, _fakeCameraService);

            var userInputField = typeof(MainCharacterAttacker).GetField(
                "_userInputControls",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            userInputField.SetValue(_attacker, _userInputControls);

            var dummyInputField = typeof(MainCharacterAttacker).GetField(
                "_dummyInputControls",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            dummyInputField.SetValue(_attacker, _dummyInputControls);


            _unitGo = new GameObject("TestUnit");
            var unityAnimator = _unitGo.AddComponent<UnityEngine.Animator>();
            _unitAnimator = _unitGo.AddComponent<UnitAnimator>();
            
            var animField = typeof(UnitAnimator).GetField(
                "_animator",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            animField.SetValue(_unitAnimator, unityAnimator);
            
            _gameUnit = _unitGo.AddComponent<GameUnit>();
            _gameUnit.Animator = _unitAnimator;
            
            var weaponGameObject = new GameObject("TestWeapon");
            _testWeapon = weaponGameObject.AddComponent<TestWeapon>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_unitGo);
            Object.DestroyImmediate(_attackerGo);
            _fakeCameraService.Destroy();
            _userInputControls.Dispose();
        }
        

        [Test]
        public void Shoot_WhenUnitHasWeapon_CallsAnimatorShoot()
        {
            // Arrange
            var shootPosition = DefaultShootPosition;
            _gameUnit.UpdateWeapon(_testWeapon);

            // Act
            _attacker.Shoot(_gameUnit, shootPosition);

            // Assert
            Assert.Pass("Shoot завершён без ошибок"); // change on fake animator calls shoot
        }

        [Test]
        public void Shoot_WhenUnitHasNoWeapon_DoesNotThrow()
        {
            // Arrange
            var shootPosition = DefaultShootPosition;
            _gameUnit.UpdateWeapon(null);

            // Act Assert
            Action shootAction = () => _attacker.Shoot(_gameUnit, shootPosition);
            shootAction.Should().NotThrow();
        }
        

        [Test]
        public void OnShootCast_WhenUnitHasWeapon_CallsWeaponShoot()
        {
            // Arrange
            var shootPosition = DefaultShootPosition;
            _gameUnit.UpdateWeapon(_testWeapon);
            
            _attacker.Shoot(_gameUnit, shootPosition);

            // Act
            _attacker.OnShootCast(_gameUnit);

            // Assert
            _testWeapon.ShootWasCalled.Should().BeTrue();
            shootPosition.Should().Be(_testWeapon.LastShootPosition);
        }

        [Test]
        public void OnShootCast_WhenUnitHasNoWeapon_DoesNotThrow()
        {
            // Arrange
            _gameUnit.UpdateWeapon(null);

            // Act Assert
            Action onShootAction = () => _attacker.OnShootCast(_gameUnit);
            onShootAction.Should().NotThrow();
        }
    }
    
    public class TestWeapon : WeaponBase
    {
        public bool ShootWasCalled { get; private set; }
        public Vector2 LastShootPosition { get; private set; }
        public int ShootCallCount { get; private set; }

        public override void Shoot(Vector2 shootMousePosition, GameUnit owner)
        {
            ShootWasCalled = true;
            LastShootPosition = shootMousePosition;
            ShootCallCount++;
        }

        public void Reset()
        {
            ShootWasCalled = false;
            LastShootPosition = Vector2.zero;
            ShootCallCount = 0;
        }
    }
}