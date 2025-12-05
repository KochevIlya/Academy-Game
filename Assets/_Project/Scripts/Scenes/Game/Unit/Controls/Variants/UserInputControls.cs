using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Scenes.Game.Unit.Controls
{
    public class UserInputControls : IInputControls, PlayerControls.IPlayerActions
    {
        private readonly Subject<Vector2> _movement = new();
        private readonly Subject<Vector2> _rotation = new();

        public IObservable<Vector2> OnMovement => _movement;
        public IObservable<Vector2> OnRotation => _rotation;

        private PlayerControls _input;
        
        public void Initialize()
        {
            if(_input == null)
            {
                _input = new PlayerControls();
                _input.Player.SetCallbacks(this);
            }
            
            _input.Enable();
        }

        public void OnMove(InputAction.CallbackContext ctx) => _movement.OnNext(ctx.ReadValue<Vector2>());
        public void OnLook(InputAction.CallbackContext ctx)  => _rotation.OnNext(ctx.ReadValue<Vector2>());
    }
}