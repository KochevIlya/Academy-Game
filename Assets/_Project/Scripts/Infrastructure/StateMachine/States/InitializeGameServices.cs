using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.StateMachine.States
{
    public class InitializeGameServices : IEnterState
    {
        private readonly IGameFactory _gameFactory;
        private readonly WeaponData _weaponData;
        
        public InitializeGameServices(/*IGameFactory gameFactory, WeaponData  data*/)
        {
            //_gameFactory = gameFactory;
            //_weaponData =  data;
        }

        public UniTask Enter(IGameStateMachine gameStateMachine)
        {
            //_gameFactory.Initialize(_weaponData.Bullet);
            Debug.Log("Initializing game services");
            
            gameStateMachine.Enter<InitializeCurrentSceneState>();
            return UniTask.CompletedTask;
        }
    }
}
