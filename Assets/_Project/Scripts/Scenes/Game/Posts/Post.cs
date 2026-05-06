using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace _Project.Scripts.Scenes.Game.Posts
{
    public class Post : MonoBehaviour
    {
        [SerializeField] private GameObject _plane;
        [SerializeField] private float _moveDownDistance = 5f;
        [SerializeField] private float _moveDuration = 0.5f;
        
        public bool _isActive = true;
        private Vector3 _originalPosition;
        private Vector3 _targetPosition;
        
        void Start()
        {
            _originalPosition = _plane.transform.position;
            _targetPosition = _originalPosition - new Vector3(0, _moveDownDistance, 0);
            
            if (!_isActive)
            {
                _plane.transform.position = _targetPosition;
            }
        }

        public void Hide()
        {
            _isActive = false;
            _plane.transform.DOKill();
            _plane.transform.DOMove(_targetPosition, _moveDuration).SetEase(Ease.InOutQuad);
        }
        
        public void Show()
        {
            _isActive = true;
            _plane.transform.DOKill();
            _plane.transform.DOMove(_originalPosition, _moveDuration).SetEase(Ease.InOutQuad);
        }
        
        public void HideImmediate()
        {
            _isActive = false;
            _plane.transform.DOKill();
    
            if (_originalPosition == Vector3.zero) 
            {
                _originalPosition = _plane.transform.position;
                _targetPosition = _originalPosition - new Vector3(0, _moveDownDistance, 0);
            }
    
            _plane.transform.position = _targetPosition;
        }
    }
}

