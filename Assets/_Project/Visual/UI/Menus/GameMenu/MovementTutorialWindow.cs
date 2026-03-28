using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Visual.UI.Menus.GameMenu
{
    public class MovementTutorialWindow : BaseScreen
    {
        private UserInputControls  _userInputControls;
        private IGuiGameService _guiService;
        private readonly HashSet<Vector2> _pressedDirections = new HashSet<Vector2>();
        
        [Inject]
        public void Construct(
            UserInputControls userInputControls
            ,IGuiGameService guiService
        )
        {
            _userInputControls = userInputControls;
            _guiService = guiService;
        }
        
        private void Start()
        {
            _userInputControls.OnMovement
                .Subscribe(direction => CheckMovement(direction))
                .AddTo(LifeTimeDisposable);
        }
        
        private void CheckMovement(Vector3 direction)
        {
            if (direction.z > 0) AddDirection(Vector2.up);
            if (direction.z < 0) AddDirection(Vector2.down);
            if (direction.x < 0) AddDirection(Vector2.left);
            if (direction.x > 0) AddDirection(Vector2.right);
        }

        private void AddDirection(Vector2 dir)
        {
            if (_pressedDirections.Add(dir))
            {
                Debug.Log($"Направлений пройдено: {_pressedDirections.Count}/4");
                
                if (_pressedDirections.Count == 4)
                {
                    FinishTutorial();
                }
            }
        }
        private void FinishTutorial()
        {
            Debug.Log("Движение освоено! Переходим к следующему этапу.");
            
            _guiService.CloseScreen(GetScreenType()).Forget();
            
            _guiService.ShowWindow(ScreenType.TutorialTerminalWindow).Forget();
        }
        
        public override ScreenType GetScreenType()
        {
            return ScreenType.TutorialMovementWindow;
        }
    }
}
