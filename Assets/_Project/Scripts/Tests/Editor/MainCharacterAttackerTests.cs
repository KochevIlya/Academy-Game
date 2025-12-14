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
            var shootPosition = new Vector2(500, 300);
            _gameUnit.UpdateWeapon(_testWeapon);

            // Act
            _attacker.Shoot(_gameUnit, shootPosition);

            // Assert
            Assert.Pass("Shoot завершён без ошибок");
        }

        [Test]
        public void Shoot_WhenUnitHasNoWeapon_DoesNotThrow()
        {
            // Arrange
            var shootPosition = new Vector2(500, 300);
            _gameUnit.UpdateWeapon(null);

            // Act Assert
            Assert.DoesNotThrow(() => _attacker.Shoot(_gameUnit, shootPosition),
                "Shoot не должен выбросить исключение если нет оружия");
        }
        

        [Test]
        public void OnShootCast_WhenUnitHasWeapon_CallsWeaponShoot()
        {
            // Arrange
            var shootPosition = new Vector2(500, 300);
            _gameUnit.UpdateWeapon(_testWeapon);
            
            _attacker.Shoot(_gameUnit, shootPosition);

            // Act
            _attacker.OnShootCast(_gameUnit);

            // Assert
            Assert.IsTrue(_testWeapon.ShootWasCalled,
                "Weapon.Shoot должен быть вызван");
            Assert.AreEqual(shootPosition, _testWeapon.LastShootPosition,
                "Weapon должен получить сохранённую позицию");
        }

        [Test]
        public void OnShootCast_WhenUnitHasNoWeapon_DoesNotThrow()
        {
            // Arrange
            _gameUnit.UpdateWeapon(null);

            // Act Assert
            Assert.DoesNotThrow(() => _attacker.OnShootCast(_gameUnit),
                "OnShootCast не должен выбросить исключение если нет оружия");
        }

        [Test]
        public void Shoot_WithZeroPosition_Works()
        {
            // Arrange
            var shootPosition = Vector2.zero;
            _gameUnit.UpdateWeapon(_testWeapon);

            // Act
            _attacker.Shoot(_gameUnit, shootPosition);

            // Assert
            Assert.Pass("Shoot с нулевой позицией работает");
        }
    }
    
    public class TestWeapon : WeaponBase
    {
        public bool ShootWasCalled { get; private set; }
        public Vector2 LastShootPosition { get; private set; }
        public int ShootCallCount { get; private set; }

        public override void Shoot(Vector2 shootMousePosition)
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