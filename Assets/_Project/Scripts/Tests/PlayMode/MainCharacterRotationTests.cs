using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Rotator;
using _Project.Scripts.Infrastructure.Gui.Camera;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Tests.PlayMode
{
    public class MainCharacterRotationTests
    {
        private DiContainer _container;
        
        [SetUp]
        public void Setup()
        {
            // Создаем контейнер вручную (без ZenjectIntegrationTestFixture)
            _container = new DiContainer();
            
            // Регистрируем mock
            _inputHelperMock = Substitute.For<IInputHelper>();
            _container.Bind<IInputHelper>().FromInstance(_inputHelperMock);
            
            // Создаем GameObject и добавляем компонент
            var rotatorGo = new GameObject("Rotator");
            _rotator = rotatorGo.AddComponent<MainCharacterRotator>();
            
            // Инжектим зависимость вручную
            _container.InjectExistingInstance(_rotator);
            
            // Создаем тестовый unit
            _gameUnit = new GameObject("TestUnit").AddComponent<GameUnit>();
        }
        
        [Test]
        public void IsUnitRotating()
        {
            var unit = new GameObject().AddComponent<GameUnit>();

            var rotator = new MainCharacterRotator();
            
            
            Assert.Pass();
        }
    }
}
