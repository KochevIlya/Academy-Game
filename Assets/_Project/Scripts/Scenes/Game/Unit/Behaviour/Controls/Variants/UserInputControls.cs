using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit.Controls.Variants
{
  public class UserInputControls : IInitializable, IDisposable, IInputControls, PlayerControls.IPlayerActions
  {
    private PlayerControls _input;
    private Vector2 _moveDirection;
    private Vector2 _gamepadAimInput;
    private readonly float _gamepadSensitivity = 1000f;

    private readonly Subject<Vector2> _movement = new Subject<Vector2>();
    private readonly Subject<UniRx.Unit> _shoot = new Subject<UniRx.Unit>();
    private readonly Subject<UniRx.Unit> _abilityUse = new Subject<UniRx.Unit>();
    private readonly CompositeDisposable _disposable = new CompositeDisposable();
    private readonly Subject<UniRx.Unit> _hackingUse = new Subject<UniRx.Unit>();
    private readonly Subject<UniRx.Unit> _cancelUse = new Subject<UniRx.Unit>();
    public BoolReactiveProperty IsBlocked { get; } = new BoolReactiveProperty(false);
    public Vector2 MousePosition { get; private set; }
    public IObservable<Vector2> OnRawMovement => _movement;
    public IObservable<Vector3> OnMovement => IsBlocked
      .Select(blocked => blocked 
        ? Observable.Return(Vector3.zero)
        : _movement.Select(vec2 => new Vector3(vec2.x, 0, vec2.y)))
      .Switch();
    public IObservable<UniRx.Unit> OnShoot => _shoot.Where(_ => !IsBlocked.Value);
    public IObservable<UniRx.Unit> OnAbilityUse => _abilityUse;
    public IObservable<UniRx.Unit> OnHacking => _hackingUse;
    public IObservable<UniRx.Unit> OnCancel => _cancelUse;


    public void Initialize()
    {
      if (_input == null)
      {
        _input = new PlayerControls();
        _input.Player.SetCallbacks(this);
      }

      _input.Enable();

      Observable.EveryUpdate()
        
        .Subscribe(_ =>
        {
          _movement.OnNext(_moveDirection);
          HandleGamepadAiming();
        })
    .AddTo(_disposable);
    }

    void PlayerControls.IPlayerActions.OnMove(InputAction.CallbackContext ctx)
    {
      _moveDirection = ctx.ReadValue<Vector2>();
    }

    void PlayerControls.IPlayerActions.OnShoot(InputAction.CallbackContext ctx)
    {
      if (ctx.phase == InputActionPhase.Performed)
        _shoot.OnNext(UniRx.Unit.Default);
    }
    public void OnMousePosition(InputAction.CallbackContext ctx) =>
      MousePosition = ctx.ReadValue<Vector2>();

    void PlayerControls.IPlayerActions.OnAbility(InputAction.CallbackContext ctx)
    {
      if (ctx.phase == InputActionPhase.Started)
        _abilityUse.OnNext(UniRx.Unit.Default);
    }

    void PlayerControls.IPlayerActions.OnHacking(InputAction.CallbackContext ctx)
    {
      if (ctx.phase == InputActionPhase.Started)
        _hackingUse.OnNext(UniRx.Unit.Default);
    }

    void PlayerControls.IPlayerActions.OnCancel(InputAction.CallbackContext ctx)
    {
      if (ctx.phase == InputActionPhase.Performed || ctx.phase == InputActionPhase.Started)
      {
        Debug.Log("Кнопка ESC нажата в UserInputControls!");
        _cancelUse.OnNext(UniRx.Unit.Default);
      }
    }
    
    private void HandleGamepadAiming()
    {
      if (_gamepadAimInput != Vector2.zero)
      {
        Vector2 delta = _gamepadAimInput * _gamepadSensitivity * Time.deltaTime;
                
        Vector2 newPos = MousePosition + delta;
        newPos.x = Mathf.Clamp(newPos.x, 0, Screen.width);
        newPos.y = Mathf.Clamp(newPos.y, 0, Screen.height);

        MousePosition = newPos;
      }
    }
    public void OnAim(InputAction.CallbackContext context)
    {
      _gamepadAimInput = context.ReadValue<Vector2>();
    }

    public void Dispose()
    {
      _disposable?.Dispose();

      _input?.Dispose();
      
    }
  }
}