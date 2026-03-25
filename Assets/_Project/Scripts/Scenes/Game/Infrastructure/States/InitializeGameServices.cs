using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Infrastructure.States
{
    public class InitializeGameServices : IEnterState
    {
        private readonly IGameFactory _gameFactory;
        private readonly IStaticDataService _staticData;
        
        public InitializeGameServices(IGameFactory gameFactory, IStaticDataService staticData)
        {
            _gameFactory = gameFactory;
            _staticData = staticData;
        }

        public async UniTask Enter(IGameStateMachine gameStateMachine)
        {
            var weaponConfigs = _staticData.WeaponsConfig.Weapons;
    
            List<UniTask> initTasks = new List<UniTask>();

            foreach (var config in weaponConfigs.Values)
            {
                if (config.Bullet != null && config.Bullet.RuntimeKeyIsValid()) 
                {
                    initTasks.Add(_gameFactory.Initialize(config.Bullet));
                }
            }
            await UniTask.WhenAll(initTasks);
    
            gameStateMachine.Enter<LoadProgressState>();
        }
    }
}
