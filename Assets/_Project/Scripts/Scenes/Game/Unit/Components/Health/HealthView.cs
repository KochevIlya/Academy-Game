using UnityEngine;
using UnityEngine.UI;
using UniRx;
using _Project.Scripts.Scenes.Game.Unit;
using TMPro;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Health
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _healthText;

        [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);

        private GameUnit _targetUnit;
        private Camera _mainCamera;
        private RectTransform _rectTransform;
        [Inject] IPlayerProvider _playerProvider;
        
        public void Initialize(GameUnit unit)
        {
            if (this == null) return;
            _targetUnit = unit;
            _mainCamera = Camera.main;
            _rectTransform = GetComponent<RectTransform>();

            UpdateHealthBar();
            _playerProvider.ActiveUnit.Subscribe(unit => UnitChecking(unit)).AddTo(this);
            unit.Health.CurrentHealth
                .Subscribe(currentHealth => UpdateHealthBar())
                .AddTo(this);
            
            unit.Health.MaxHealth
                .Subscribe(currentHealth => UpdateHealthBar())
                .AddTo(this);
            
            unit.Health.Die
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
        }

        private void UnitChecking(GameUnit unit)
        {
            gameObject.SetActive(unit != _targetUnit);
        }
        private void UpdateHealthBar()
        {
            int currentHealth = _targetUnit.Health.CurrentHealth.Value;
            int maxHealth = _targetUnit.Health.MaxHealth.Value;
            if (_targetUnit.Health.MaxHealth.Value <= 0) return;
            
            float fillAmount = (float)currentHealth / maxHealth;
            _fillImage.fillAmount = fillAmount;
            
            // if (_healthText != null)
            //     _healthText.text = $"{currentHealth} / {maxHealth}";
            
        }

        public void SetVisible(bool isVisible)
        {
            _fillImage.gameObject.SetActive(isVisible);
            return;
        }
        private void FixedUpdate()
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