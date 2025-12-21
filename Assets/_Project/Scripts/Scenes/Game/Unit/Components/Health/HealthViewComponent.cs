using UnityEngine;
using UnityEngine.UI;
using UniRx;
using _Project.Scripts.Scenes.Game.Unit;
using TMPro;

namespace _Project.Scripts.UI
{
    public class HealthViewComponent : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _healthText;

        [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);

        private GameUnit _targetUnit;
        private Camera _mainCamera;
        private RectTransform _rectTransform;

        public void Initialize(GameUnit unit)
        {
            _targetUnit = unit;
            _mainCamera = Camera.main;
            _rectTransform = GetComponent<RectTransform>();

            UpdateHealthBar(unit.Health.CurrentHealth.Value);

            unit.Health.CurrentHealth
                .Subscribe(currentHealth => UpdateHealthBar(currentHealth))
                .AddTo(this);
            
            unit.Health.Die
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
        }

        private void UpdateHealthBar(int currentHealth)
        {
            if (_targetUnit.Health.MaxHealth <= 0) return;
            
            float fillAmount = (float)currentHealth / _targetUnit.Health.MaxHealth;
            _fillImage.fillAmount = fillAmount;
            
            if (_healthText != null)
                _healthText.text = $"{currentHealth} / {_targetUnit.Health.MaxHealth}";
            
        }

        private void LateUpdate()
        {
            if (_targetUnit == null)
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