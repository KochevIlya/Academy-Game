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
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HackingService : IDisposable
{
    private readonly UserInputControls _input;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly DiContainer _container;
    
    [Inject] HackableSelector _hackableSelector;
    
    private List<GameUnit> _availableTargets;
    public ReactiveProperty<bool> IsHacking { get; } = new ReactiveProperty<bool>(false);
    public ReactiveProperty<int> CurrentProgressIndex { get; } = new ReactiveProperty<int>(0);
    public Subject<List<Vector2>> OnHackingStarted { get; } = new Subject<List<Vector2>>();
    public Subject<int> OnError { get; } = new Subject<int>();
    public Subject<bool> OnHackingFinished { get; } = new Subject<bool>();
    public bool IsPossessing => _isPossessing;
    public ReactiveProperty<bool> CanHack { get; } = new ReactiveProperty<bool>(false);
    public Subject<HackableComponent> OnHackingProcessStarted { get; } = new Subject<HackableComponent>();
    
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

    
    public HackingService(UserInputControls input, DiContainer container)
    {
        _input = input;
        _container = container;
        SubscribeToInput();
    }
    public void SetHackingZoneStatus(bool isInZone)
    {
        CanHack.Value = isInZone;
    }
    public async void RequestHacking(GameUnit hacker)
    {
        if (!CanHack.Value) return;
        if (IsHacking.Value) return;

        _hackerUnit = hacker;
        _originalHero ??= hacker;

        _input.IsBlocked.Value = true;
        var dummy = _container.Resolve<DummyInputControls>();
    
        HackableComponent target = null;

        if (CanHack.Value)
        {
            _cursorService.SetDefaultCursor();
            _hackerUnit.DisableControl(dummy);
            target = await _hackableSelector.SelectTarget(new CancellationTokenSource().Token);
        }

        if (target != null)
        {
            StartHacking(target, hacker);
        }
        else
        {
            _input.IsBlocked.Value = false;
            _hackerUnit.UpdateControls(_input);
            _cameraService.SetTarget(_originalHero);
        }
    }
    private HackableComponent FindClosestTargetInWorld(Vector3 position)
    {
        float radius = 10f;
        var allTargets = UnityEngine.Object.FindObjectsByType<HackableComponent>(FindObjectsSortMode.None);
        return allTargets
            .Where(t => t.gameObject != _hackerUnit.gameObject)
            .Select(t => new { Component = t, Distance = Vector3.Distance(position, t.transform.position) })
            .Where(t => t.Distance <= radius)
            .OrderBy(t => t.Distance)
            .Select(t => t.Component)
            .FirstOrDefault();
    }
    public void StartHacking(HackableComponent target, GameUnit hacker)
    {
        _currentTarget = target;
        _currentSequence = GenerateSequence(target.Difficulty);
        _waitForRelease = false;
    
        CurrentProgressIndex.Value = 0;
        IsHacking.Value = true;
    
        _input.IsBlocked.Value = true;
        _cameraService.SetTarget(target.GetComponent<GameUnit>());
    
        OnHackingStarted.OnNext(_currentSequence);
        
        OnHackingProcessStarted.OnNext(target);
        OnHackingStarted.OnNext(_currentSequence);
    }

    private void SubscribeToInput()
    {
        _input.OnRawMovement
            .Where(_ => IsHacking.Value)
            .Subscribe(CheckInput)
            .AddTo(_disposables);
        
        _input.OnCancel
            .Where(_ => _isPossessing && !IsHacking.Value)
            .Subscribe(_ => ReturnToOriginalBody())
            .AddTo(_disposables);
    }
    public void ReturnToOriginalBody()
    {
        if (_originalHero == null) 
        {
            Debug.LogError("Ошибка возврата: оригинальное тело хакера потеряно!");
            return;
        }
        _cameraService.SetTarget(_originalHero);

        if (_currentPossessedUnit != null)
        {
            var dummy = _container.Resolve<DummyInputControls>();
            _currentPossessedUnit.DisableControl(dummy);
        }

        _originalHero.UpdateControls(_input);

        _isPossessing = false;
        _currentPossessedUnit = null;
        _hackerUnit = _originalHero;
    
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
        Debug.Log($"Взлом {_currentTarget.name} успешен!");
        GameUnit victimUnit = _currentTarget.GetComponent<GameUnit>();
    
        if (victimUnit != null && _hackerUnit != null)
        {
            int targetLayer = LayerMask.NameToLayer("Default"); 
        
            if (targetLayer != -1)
            {
                SetLayerRecursively(victimUnit.gameObject, targetLayer);
            }
            
            _input.IsBlocked.Value = false;

            var dummy = _container.Resolve<DummyInputControls>();
            _isPossessing = true;
            _currentPossessedUnit = victimUnit;
        
            _hackerUnit.DisableControl(dummy);
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
        OnHackingFinished.OnNext(true);
        IsHacking.Value = false;
        _currentTarget = null;
    }
    
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
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
    public void Dispose()
    {
        _disposables.Dispose();
    }
}