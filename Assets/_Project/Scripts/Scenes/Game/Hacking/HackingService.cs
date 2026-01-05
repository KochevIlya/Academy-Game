using System;
using System.Collections;
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
        private IInputControls _input;
        private ICameraService _cameraService;
    
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
        private List<Vector2> _currentSequence = new List<Vector2>();
        private int _currentIndex;
        private bool _isHacking;
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
            _currentTarget = target;
            _isHacking = true;
            IsHackingProcess.Value = true;
            
            GenerateSequence(target.Difficulty);
            _view.Show(_currentSequence);
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

        private void SubscribeToInput()
        {
            _input.OnMovement 
                .Where(_ => _isHacking)
                .Subscribe(inputDir => CheckInput(inputDir))
                .AddTo(_disposables);
        }

        private void CheckInput(Vector2 input)
        {
            if (input == Vector2.zero) return; 
            
            Vector2 cardinal = Vector2.zero;
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                cardinal = input.x > 0 ? Vector2.right : Vector2.left;
            else
                cardinal = input.y > 0 ? Vector2.up : Vector2.down;

            if (cardinal == _currentSequence[_currentIndex])
            {
                _currentIndex++;
                _view.UpdateProgress(_currentIndex);

                if (_currentIndex >= _currentSequence.Count)
                {
                    CompleteHacking();
                }
            }
            else
            {
                _view.ShowError();
            }
        }

        private void CompleteHacking()
        {
            _isHacking = false;
            IsHackingProcess.Value = false;
            _view.Hide();
            
            Debug.Log($"Взлом {_currentTarget.name} успешен!");
        }
        
        public void StopHacking()
        {
             _isHacking = false;
             IsHackingProcess.Value = false;
             _view.Hide();
        }

        public void Dispose()
        {
            _disposables.Clear();
        }
    }
