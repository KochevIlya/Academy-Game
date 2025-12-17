using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Libs.Pool;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.StateMachine.States
{
    public class InitializeGameServices : IEnterState
    {
        //private readonly IGameFactory _objectPool;
        //private readonly WeaponData _weaponData;
        
        public InitializeGameServices(/*IGameFactory gameFactory, WeaponData  data*/)
        {
            //_objectPool = gameFactory;
            //_weaponData =  data;
        }

        public UniTask Enter(IGameStateMachine gameStateMachine)
        {
            //_objectPool.Initialize(_weaponData.Bullet);
            
            return UniTask.CompletedTask;
        }
    }
}
