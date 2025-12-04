using System;
using UniRx;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = System.Numerics.Vector2;

namespace _Project.Scripts.Scenes.Game.Unit.Controls
{
    public class UserInputControls : IInputControls
    {
        private readonly Subject<Vector2> _movement = new();
        private readonly Subject<Vector2> _rotation = new();

        public IObservable<Vector2> OnMovement => _movement;
        public IObservable<Vector2> OnRotation => _rotation;

        private PlayerControls _input;

        public UserInputControls()
        {
            _input = new PlayerControls();
            _input.Player.Move.performed += OnMove;
            _input.Player.Move.canceled  += OnMove;
            
            _input.Player.Look.performed += OnLook;
            _input.Player.Look.canceled  += OnLook;

            _input.Enable();
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            UnityEngine.Vector2 unityValue = ctx.ReadValue<UnityEngine.Vector2>();
            var v = new Vector2(unityValue.x, unityValue.y);
            _movement.OnNext(v);
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            UnityEngine.Vector2 unityValue = ctx.ReadValue<UnityEngine.Vector2>();
            var v = new Vector2(unityValue.x, unityValue.y);

            _rotation.OnNext(v);
        }
    }
}