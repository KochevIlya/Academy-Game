using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Camera;
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
    public ReactiveProperty<bool> IsHacking { get; } = new ReactiveProperty<bool>(false);
    public ReactiveProperty<int> CurrentProgressIndex { get; } = new ReactiveProperty<int>(0);
    public Subject<List<Vector2>> OnHackingStarted { get; } = new Subject<List<Vector2>>();
    public Subject<int> OnError { get; } = new Subject<int>();
    public Subject<bool> OnHackingFinished { get; } = new Subject<bool>();

    private List<Vector2> _currentSequence;
    private HackableComponent _currentTarget;
    private bool _waitForRelease;

    public HackingService(UserInputControls input)
    {
        _input = input;
        SubscribeToInput();
    }

    public void StartHacking(HackableComponent target)
    {
        _currentTarget = target;
        _currentSequence = GenerateSequence(target.Difficulty);
        _waitForRelease = false;
        
        CurrentProgressIndex.Value = 0;
        IsHacking.Value = true;

        OnHackingStarted.OnNext(_currentSequence);
    }

    public void StopHacking()
    {
        IsHacking.Value = false;
        OnHackingFinished.OnNext(false);
    }

    private void SubscribeToInput()
    {
        _input.OnMovement
            .Where(_ => IsHacking.Value)
            .Subscribe(CheckInput)
            .AddTo(_disposables);
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
        int index = CurrentProgressIndex.Value;
        
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
            OnError.OnNext(index);
        }
    }

    private void CompleteHacking()
    {
        Debug.Log($"Взлом {_currentTarget.name} успешен!");
        
        Observable.Timer(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ =>
            {
                StopHacking();
            });
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