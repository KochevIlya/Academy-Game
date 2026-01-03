using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Infrastructure.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Infrastructure.States
{
    public class InitializeGameServices : IEnterState
    {
        private readonly IGameFactory _gameFactory;
        private readonly WeaponData _weaponData;
        
        public InitializeGameServices(IGameFactory gameFactory/*, WeaponData  data*/)
        {
            _gameFactory = gameFactory;
            //_weaponData =  data;
        }

        public async UniTask Enter(IGameStateMachine gameStateMachine)
        {
            //await _gameFactory.Initialize(_weaponData.Bullet);
            Debug.Log("Initializing game services");
            
            gameStateMachine.Enter<GameLoopState>().Forget();
        }
    }
}
