using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class HackingService : MonoBehaviour
{
    [SerializeField] private HackingView _view;
    private UserInputControls _input; 
    private ICameraService _cameraService;

    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private List<Vector2> _currentSequence = new List<Vector2>();
    private int _currentIndex;
    private bool _isHacking;
    
    private bool _waitForRelease; 

    private HackableComponent _currentTarget;
    public ReactiveProperty<bool> IsHackingProcess { get; } = new ReactiveProperty<bool>(false);

    [Inject]
    public void Construct(UserInputControls input, ICameraService cameraService)
    {
        _input = input;
        _cameraService = cameraService;
        SubscribeToInput();
    }

    public void StartHacking(HackableComponent target)
    {
        if (_view == null) return;

        _currentTarget = target;
        _isHacking = true;
        _waitForRelease = false;
        IsHackingProcess.Value = true;

        GenerateSequence(target.Difficulty);
        _view.Show(_currentSequence);
        _view.UpdateProgress(0);
    }

    private void SubscribeToInput()
    {
        if (_input == null) return;

        _input.OnMovement
            .Where(_ => _isHacking)
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

        if (cardinal == _currentSequence[_currentIndex])
        {
            _currentIndex++;
            
            if (_currentIndex >= _currentSequence.Count)
            {
                _view.UpdateProgress(_currentIndex); 
                CompleteHacking();
            }
            else
            {
                _view.UpdateProgress(_currentIndex);
            }
        }
        else
        {
            _view.ShowError(_currentIndex);
        }
    }

    private void GenerateSequence(int length)
    {
        _currentSequence = new List<Vector2>();
        _currentIndex = 0;
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
            _currentSequence.Add(dir);
        }
    }

    private void CompleteHacking()
    {
        Debug.Log($"Взлом {_currentTarget.name} успешен!");
        
        Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => 
            {
                _isHacking = false;
                IsHackingProcess.Value = false;
                _view.Hide();
            }).AddTo(_disposables);
    }

    public void StopHacking()
    {
        _isHacking = false;
        IsHackingProcess.Value = false;
        _view.Hide();
    }
}