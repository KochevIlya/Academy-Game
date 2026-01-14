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
            await _gameFactory.Initialize(_staticData.WeaponsConfig.Weapons[WeaponType.Riffle].Bullet);
            
            gameStateMachine.Enter<GameLoopState>().Forget();
        }
    }
}
