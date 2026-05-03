using UnityEngine;
using TMPro;
using UniRx;
using Zenject;
using _Project.Scripts.Scenes.Game.Unit;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Timer
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);

        private GameUnit _targetUnit;
        private Camera _mainCamera;
        private RectTransform _rectTransform;
        [Inject] private IPlayerProvider _playerProvider;

        public void Initialize(GameUnit unit)
        {
            if (this == null) return;
        
            _targetUnit = unit;
            _mainCamera = Camera.main;
            _rectTransform = GetComponent<RectTransform>();

            unit.Health.Die
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
        }

        private void UnitChecking(GameUnit activeUnit)
        {
            gameObject.SetActive(activeUnit != _targetUnit);
        }

        public void UpdateTimerText(string text)
        {
            if (_timerText != null)
                _timerText.text = text;
        }

        private void LateUpdate()
        {
            if (_targetUnit == null || _targetUnit.gameObject == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 targetPosition = _targetUnit.transform.position + _offset;
            Vector3 screenPoint = _mainCamera.WorldToScreenPoint(targetPosition);
        
            bool isBehind = screenPoint.z < 0; 
            _canvasGroup.alpha = isBehind ? 0 : 1;

            if (!isBehind)
                _rectTransform.position = screenPoint;
        }
    }
}