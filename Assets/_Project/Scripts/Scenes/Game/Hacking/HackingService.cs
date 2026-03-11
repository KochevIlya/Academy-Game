using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Hacking;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HackingService : IDisposable
{
    private  UserInputControls _input;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private  DiContainer _container;
    
    [Inject] HackableSelector _hackableSelector;
    
    private List<GameUnit> _availableTargets;
    public ReactiveProperty<bool> IsHacking { get; } = new ReactiveProperty<bool>(false);
    public ReactiveProperty<int> CurrentProgressIndex { get; } = new ReactiveProperty<int>(0);
    public Subject<List<Vector2>> OnHackingStarted { get; } = new Subject<List<Vector2>>();
    public Subject<int> OnError { get; } = new Subject<int>();
    public bool IsPossessing => _isPossessing;
    public ReactiveProperty<bool> CanHack { get; } = new ReactiveProperty<bool>(false);
    public Subject<HackableComponent> OnHackingProcessStarted { get; } = new Subject<HackableComponent>();
    private UniTaskCompletionSource _hackingCompletionSource;
    private List<Vector2> _currentSequence;
    private HackableComponent _currentTarget;
    [Inject] private ICameraService _cameraService;
    [Inject] private ICursorService _cursorService;
    private bool _waitForRelease;
    private GameUnit _hackerUnit;
    private GameUnit _originalHero;
    private GameUnit _currentPossessedUnit;
    private bool _isPossessing;
    
    private bool _isErrorState;
    private CancellationTokenSource _hackingCts;
    private CombatZone _currentZoneContext;
    public HackingService(UserInputControls input, DiContainer container)
    {
        _input = input;
        _container = container;
        SubscribeToInput();
        
    }
    public void SetCurrentZoneContext(CombatZone zone)
    {
        _currentZoneContext = zone;
    }
    public void SetHackingZoneStatus(bool isInZone)
    {
        CanHack.Value = isInZone;
    }
    public void RequestCancel(bool silent = false)
    {
        _hackingCts?.Cancel();
    
        if (!silent)
        {
            ReturnToOriginalBody();
        }
        else
        {
            IsHacking.Value = false;
            _input.IsBlocked.Value = false;
        }
    }
    public async UniTask RequestHacking(GameUnit hacker)
    {
        
        if (!CanHack.Value) return;
        if (IsHacking.Value) return;
        
        _hackingCompletionSource = new UniTaskCompletionSource();
        _hackingCts?.Cancel();
        _hackingCts = new CancellationTokenSource();
        
        
        _hackerUnit = hacker;
        _originalHero ??= hacker;

        _input.IsBlocked.Value = true;
    
        HackableComponent target = null;
        
        OnHackingProcessStarted.OnNext(null);
        _cameraService.ZoomOut();

        try
        {
            _cursorService.SetDefaultCursor();
            _hackerUnit.DisableControl();

            target = await _hackableSelector.SelectTarget(_hackingCts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[HackingService] Выбор цели отменен.");
            return;
        }
        catch (System.Exception e)
        {
            _hackingCts?.Cancel();
            Debug.LogError($"Непредвиденная ошибка при выборе цели: {e}");
            return;
        }
        finally
        {
            _hackingCompletionSource?.TrySetResult(); 
            _hackingCompletionSource = null;
        }

        if (target != null)
        {
            StartHacking(target, hacker);
        }
        else
        {
            ReturnToOriginalBody();
        }
    }
    public UniTask WaitUntilFinished() => _hackingCompletionSource?.Task ?? UniTask.CompletedTask;
    public void StartHacking(HackableComponent target, GameUnit hacker)
    {
        _currentTarget = target;
        int length = target.Difficulty;
        if (_currentZoneContext != null)
        {
            length = _currentZoneContext.GetNextSequenceLength(length);
        }
        _currentSequence = GenerateSequence(length);
        _waitForRelease = false;
    
        CurrentProgressIndex.Value = 0;
        IsHacking.Value = true;
    
        _input.IsBlocked.Value = true;
        _cameraService.SetTarget(target.GetComponent<GameUnit>());
    
        OnHackingStarted.OnNext(_currentSequence);
    }

    private void SubscribeToInput()
    {
        _input.OnRawMovement
            .Where(_ => IsHacking.Value)
            .Subscribe(CheckInput)
            .AddTo(_disposables);
        
        _input.OnAction
            .Where(_ => _isPossessing && !IsHacking.Value)
            .Subscribe(_ => SelfDestroy())
            .AddTo(_disposables);
    }

    private void SelfDestroy()
    {
        var currentUnit = _currentPossessedUnit.GetComponentInChildren<GameUnit>();
        currentUnit.Health.TakeDamage(Int32.MaxValue);
        ReturnToOriginalBody();
    }
    public void ReturnToOriginalBody()
    {
        if (_originalHero == null) 
        {
            Debug.LogError("Ошибка возврата: оригинальное тело хакера потеряно!");
            return;
        }

        if (_cameraService != null)
        {
            _cameraService.SetTarget(_originalHero);
            _cameraService.ResetZoom();
        }

        if (_currentPossessedUnit != null)
        {
            _currentPossessedUnit.DisableControl();
        }

        _originalHero.UpdateControls(_input);
    
        _isPossessing = false;
        _currentPossessedUnit = null;
        _input.IsBlocked.Value = false;
        _hackerUnit = _originalHero;
        
        _cursorService.SetCrosshairCursor();
        _cursorService.SetVisible(true);
        _cursorService.SetLockState(false);
        _currentZoneContext = null;
        Debug.Log("Сознание вернулось в хакера после смерти носителя.");
    }
    private void CheckInput(Vector2 input)
    {
        if (input == Vector2.zero)
        {
            _waitForRelease = false;
            return;
        }

        if (_waitForRelease) return;

        Vector2 cardinal = Vector2.zero;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            cardinal = input.x > 0 ? Vector2.right : Vector2.left;
        else
            cardinal = input.y > 0 ? Vector2.up : Vector2.down;

        _waitForRelease = true;

        ValidateStep(cardinal);
    }

    private void ValidateStep(Vector2 inputDir)
    {
        if (_isErrorState) return;
        if (!IsHacking.Value) return;
        
        int index = CurrentProgressIndex.Value;
        
        if (index >= _currentSequence.Count) 
        {
            return; 
        }
        if (inputDir == _currentSequence[index])
        {
            index++;
            CurrentProgressIndex.Value = index;

            if (index >= _currentSequence.Count)
            {
                CompleteHacking();
            }
        }
        else
        {
            ExecuteErrorState();
        }
    }
    private void ExecuteErrorState()
    {
        _isErrorState = true;
        
        OnError.OnNext(-1); 

        Observable.Timer(TimeSpan.FromSeconds(1f))
            .Subscribe(_ => 
            {
                CurrentProgressIndex.Value = 0;
                _isErrorState = false;
                
                OnError.OnNext(-2); 
                StartHacking(_currentTarget, _hackerUnit);
            })
            .AddTo(_disposables);
        
    }
    private void CompleteHacking()
    {
        _cameraService.ResetZoom();
        if (_currentTarget == null)
        {
            
            ReturnToOriginalBody();
            IsHacking.Value = false;
            return;
        }
        Debug.Log($"Взлом {_currentTarget.name} успешен!");
        GameUnit victimUnit = _currentTarget.GetComponent<GameUnit>();
    
        if (victimUnit != null && _hackerUnit != null)
        {
            _input.IsBlocked.Value = false;

            _isPossessing = true;
            _currentPossessedUnit = victimUnit;
        
            _hackerUnit.DisableControl();
            victimUnit.UpdateControls(_input);
            victimUnit.IsUnderControl = true;
            victimUnit.OnUnitHacked.OnNext(victimUnit);

            _cameraService.SetTarget(victimUnit);

            var newHackerLogic = victimUnit.gameObject.GetComponent<PlayerHacker>() 
                                 ?? victimUnit.gameObject.AddComponent<PlayerHacker>();
            _container.Inject(newHackerLogic);
            

        }
        
        _cursorService.SetCrosshairCursor();
        _cursorService.SetVisible(true);
        _cursorService.SetLockState(false);
        
        IsHacking.Value = false;
        _currentTarget = null;
    }

    private List<Vector2> GenerateSequence(int length)
    {
        var seq = new List<Vector2>();
        for (int i = 0; i < length; i++)
        {
            int rand = Random.Range(0, 4);
            Vector2 dir = Vector2.zero;
            switch (rand)
            {
                case 0: dir = Vector2.up; break;
                case 1: dir = Vector2.down; break;
                case 2: dir = Vector2.left; break;
                case 3: dir = Vector2.right; break;
            }
            seq.Add(dir);
        }
        return seq;
    }

    public void ClearState()
    {
        _hackingCts?.Cancel();
        _hackingCts = new CancellationTokenSource();

        _isPossessing = false;
        IsHacking.Value = false;
        _isErrorState = false;
        _waitForRelease = false;
    
        CurrentProgressIndex.Value = 0;
        CanHack.Value = false;

        _currentTarget = null;
        _hackerUnit = null;
        _originalHero = null;
        _currentPossessedUnit = null;
        _currentZoneContext = null;

        _currentSequence?.Clear();

        Debug.Log("[HackingService] Состояние полностью очищено для новой загрузки.");
    }
    public void Dispose()
    {
        _disposables.Dispose();
    }
}