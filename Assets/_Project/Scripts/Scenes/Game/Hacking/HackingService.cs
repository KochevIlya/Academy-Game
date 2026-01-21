using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HackingService : IDisposable
{
    private readonly UserInputControls _input;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly DiContainer _container;
    public ReactiveProperty<bool> IsHacking { get; } = new ReactiveProperty<bool>(false);
    public ReactiveProperty<int> CurrentProgressIndex { get; } = new ReactiveProperty<int>(0);
    public Subject<List<Vector2>> OnHackingStarted { get; } = new Subject<List<Vector2>>();
    public Subject<int> OnError { get; } = new Subject<int>();
    public Subject<bool> OnHackingFinished { get; } = new Subject<bool>();
    public bool IsPossessing => _isPossessing;

    private List<Vector2> _currentSequence;
    private HackableComponent _currentTarget;
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

    public void StartHacking(HackableComponent target, GameUnit hacker)
    {
        if (_originalHero == null) 
        {
            _originalHero = hacker;
        }
        _hackerUnit = hacker;
        _currentTarget = target;
        _currentSequence = GenerateSequence(target.Difficulty);
        _waitForRelease = false;
        
        CurrentProgressIndex.Value = 0;
        IsHacking.Value = true;
        _input.IsBlocked.Value = true;
        hacker.DisableControl(_container.Resolve<DummyInputControls>());
        OnHackingStarted.OnNext(_currentSequence);
    }

    public void StopHacking()
    {
        _input.IsBlocked.Value = false;
        IsHacking.Value = false;
        OnHackingFinished.OnNext(false);
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
    private void ReturnToOriginalBody()
    {
        if (_originalHero == null || _currentPossessedUnit == null) 
        {
            Debug.LogError("Ошибка возврата: не найден герой или текущее тело!");
            return;
        }
        Debug.Log("Возврат в оригинальное тело...");
        var dummy = _container.Resolve<DummyInputControls>();

        _currentPossessedUnit.DisableControl(dummy);

        _originalHero.UpdateControls(_input);

        _isPossessing = false;
        _currentPossessedUnit = null;
        _hackerUnit = _originalHero;
        Debug.Log("Сознание вернулось в оригинальное тело.");
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
            })
            .AddTo(_disposables);
    }
    private void CompleteHacking()
    {
        Debug.Log($"Взлом {_currentTarget.name} успешен!");
        GameUnit victimUnit = _currentTarget.GetComponent<GameUnit>();

        if (victimUnit != null && _hackerUnit != null)
        {
            var dummy = _container.Resolve<DummyInputControls>();
            _isPossessing = true;
            _currentPossessedUnit = victimUnit;
            
            _hackerUnit.DisableControl(dummy);
            victimUnit.UpdateControls(_input);

            var newHackerLogic = victimUnit.gameObject.GetComponent<PlayerHacker>();
            if (newHackerLogic == null)
            {
                newHackerLogic = victimUnit.gameObject.AddComponent<PlayerHacker>();
                _container.Inject(newHackerLogic);
            }
        }

        OnHackingFinished.OnNext(true);
        
        Observable.Timer(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => StopHacking());
        _input.IsBlocked.Value = false;
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