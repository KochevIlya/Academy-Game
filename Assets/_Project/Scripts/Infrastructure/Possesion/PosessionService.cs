using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Possesion
{
    public class PosessionService : IPosessionService
    {
        private readonly IPlayerProvider _playerProvider;
        private  UserInputControls _input;
    
        [Inject]
        public PosessionService(
            IPlayerProvider playerProvider,
            UserInputControls  userInputControls
        )
        {
            _playerProvider = playerProvider;
            _input = userInputControls;
        }
    
        public void Possess(GameUnit newUnit)
        {
            if (_playerProvider.ActiveUnit.Value != null)
            {
                _playerProvider.ActiveUnit.Value.DisableControl();
            }

            _playerProvider.SetActiveUnit(newUnit);

            newUnit.UpdateControls(_input);
        
            Debug.Log($"Теперь мы управляем: {newUnit.name}");
            UpdateBlocking(false);
        }

        public void UpdateBlocking(bool blocking)
        {
            _input.IsBlocked.Value = blocking;
        }
    }
}
