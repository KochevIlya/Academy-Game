using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Scenes.Game.Unit.Controls
{
    public class UserInputControls : IInputControls, PlayerControls.IPlayerActions, IDisposable
    {
        private PlayerControls _input;
        private Vector2 _moveDirection;

        private readonly Subject<Vector2> _movement = new();
        private readonly Subject<UniRx.Unit> _shoot = new();
        private readonly Subject<UniRx.Unit> _abilityUse = new();
        private readonly CompositeDisposable _disposable = new();

        public Vector2 MousePosition { get; private set; }

        public IObservable<Vector2> OnMovement => _movement;
        public IObservable<UniRx.Unit> OnShoot => _shoot;
        public IObservable<UniRx.Unit> OnAbilityUse => _abilityUse;


        public void Initialize()
        {
            if(_input == null)
            {
                _input = new PlayerControls();
                _input.Player.SetCallbacks(this);
            }
            
            _input.Enable();
            
            Observable.EveryUpdate()
                .Subscribe(_ => _movement.OnNext(_moveDirection))
                .AddTo(_disposable);
        }

        void PlayerControls.IPlayerActions.OnMove(InputAction.CallbackContext ctx) => 
            _moveDirection = ctx.ReadValue<Vector2>();

        void PlayerControls.IPlayerActions.OnShoot(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed) 
                _shoot.OnNext(UniRx.Unit.Default);
        }
        public void OnMousePosition(InputAction.CallbackContext ctx) => 
            MousePosition = ctx.ReadValue<Vector2>();

        void PlayerControls.IPlayerActions.OnAbility(InputAction.CallbackContext ctx)
        {
            if(ctx.phase == InputActionPhase.Started)
                _abilityUse.OnNext(UniRx.Unit.Default);
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
            
            _input?.Dispose();
            _movement?.Dispose();
            _shoot?.Dispose();
            _abilityUse?.Dispose();
        }
    }
}