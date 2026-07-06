using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ArrowsMiniGame : IHackingGame
{
    private int difficulty = 4; //amount of arrows
    
    
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    public ReactiveProperty<List<Vector2>> OnHackingStarted { get; } = new ReactiveProperty<List<Vector2>>();
    private bool _waitForRelease;
    private bool _isErrorState;

    public ReactiveProperty<int> CurrentProgressIndex { get; } = new ReactiveProperty<int>(0);
    public Subject<int> OnError { get; } = new Subject<int>();
    
    
    private List<Vector2> _currentSequence;
    
    private IInputControls _input; 
    private Subject<Unit> _onComplete;
    
    [Inject]
    public ArrowsMiniGame(
        IInputControls input
    )
    {
        _input = input;
    }
    
    public IObservable<Unit> StartHackingGame()
    {
        
        _onComplete = new Subject<Unit>();
        
        SubscribeInput();
        ResetLevel();
        
        return _onComplete;
    }

    private void SubscribeInput()
    {
        _input.OnRawMovement
            .Subscribe(CheckInput)
            .AddTo(_disposables);
    }

    public void StopHackingGame()
    {
        _disposables.Clear();
        
    }
    
    private void ResetLevel()
    {
        _isErrorState = false;
        _waitForRelease = false;
        CurrentProgressIndex.Value = 0;
        
        _currentSequence = GenerateSequence(difficulty);
        Debug.Log("Implementing OnHackingStarted");
        OnHackingStarted.Value = _currentSequence;
    }
    
    private void CheckInput(Vector2 input)
    {
        if (input == Vector2.zero)
        {
            _waitForRelease = false;
            return;
        }
    
        if (_waitForRelease || _isErrorState) return;
    
        Vector2 cardinal = Vector2.zero;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            cardinal = input.x > 0 ? Vector2.right : Vector2.left;
        else
            cardinal = input.y > 0 ? Vector2.up : Vector2.down;
    
        _waitForRelease = true;
    
        ValidateStep(cardinal);
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
                ResetLevel();
            })
            .AddTo(_disposables);
        
    }   
    
    private void ValidateStep(Vector2 inputDir)
    {
        
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
                WinGame();
            }
        }
        else
        {
            ExecuteErrorState();
        }
    }

    private void WinGame()
    {
        _onComplete.OnNext(Unit.Default);
        _onComplete.OnCompleted();
        
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
    
}
