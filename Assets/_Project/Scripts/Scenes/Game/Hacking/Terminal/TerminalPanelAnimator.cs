using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Hacking.Terminal
{
    /// <summary>
    /// Анимация панели над терминалом: исчезновение/появление и подпрыгивание.
    /// </summary>
    public class TerminalPanelAnimator : MonoBehaviour
    {
        [Header("Fade Animation (Исчезновение/Появление)")]
        [Tooltip("Длительность одного цикла fade (в секундах)")]
        [SerializeField] private float fadeDuration = 1.5f;
        
        [Tooltip("Пауза между циклами fade (в секундах)")]
        [SerializeField] private float fadeWaitTime = 0.5f;
        
        [Tooltip("Минимальная прозрачность (0 = полностью прозрачный)")]
        [Range(0f, 1f)]
        [SerializeField] private float minAlpha = 0.3f;
        
        [Tooltip("Максимальная прозрачность (1 = полностью видимый)")]
        [Range(0f, 1f)]
        [SerializeField] private float maxAlpha = 1f;

        [Header("Float Animation (Подпрыгивание)")]
        [Tooltip("Длительность одного цикла подпрыгивания (в секундах)")]
        [SerializeField] private float floatDuration = 2f;
        
        [Tooltip("Амплитуда подпрыгивания (в пикселях)")]
        [SerializeField] private float floatHeight = 15f;
        
        [Tooltip("Пауза перед началом подпрыгивания (в секундах)")]
        [SerializeField] private float floatDelay = 0f;

        [Header("Настройки")]
        [Tooltip("Запускать анимации при старте")]
        [SerializeField] private bool playOnStart = true;
        
        [Tooltip("Использовать локальные координаты для подпрыгивания")]
        [SerializeField] private bool useLocalPosition = false;

        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector2 _originalAnchoredPosition;
        private Vector3 _originalLocalPosition;
        
        private Tween _fadeTween;
        private Tween _floatTween;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            
            // Если CanvasGroup не найден на этом объекте, ищем у родителя или добавляем
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponentInParent<CanvasGroup>();
            }
            
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            if (playOnStart)
            {
                PlayAllAnimations();
            }
        }

        /// <summary>
        /// Запускает все анимации.
        /// </summary>
        public void PlayAllAnimations()
        {
            PlayFadeAnimation();
            PlayFloatAnimation();
        }

        /// <summary>
        /// Запускает анимацию исчезновения/появления.
        /// </summary>
        public void PlayFadeAnimation()
        {
            // Останавливаем предыдущую анимацию, если есть
            _fadeTween?.Kill();
            
            // Устанавливаем начальное значение
            _canvasGroup.alpha = maxAlpha;
            
            // Создаём цикл fade in/out
            _fadeTween = DOVirtual.Float(
                maxAlpha, 
                minAlpha, 
                fadeDuration, 
                SetAlpha
            )
            .SetDelay(fadeWaitTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetTarget(gameObject);
        }

        /// <summary>
        /// Запускает анимацию подпрыгивания.
        /// </summary>
        public void PlayFloatAnimation()
        {
            // Останавливаем предыдущую анимацию, если есть
            _floatTween?.Kill();
            
            if (useLocalPosition)
            {
                // Анимация через localPosition
                _originalLocalPosition = transform.localPosition;
                
                Vector3 targetPos = _originalLocalPosition + Vector3.up * floatHeight;
                
                _floatTween = transform
                    .DOLocalMoveY(targetPos.y, floatDuration)
                    .SetDelay(floatDelay)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetTarget(gameObject);
            }
            else
            {
                // Анимация через anchoredPosition (для UI)
                _originalAnchoredPosition = _rectTransform.anchoredPosition;
                
                Vector2 targetPos = _originalAnchoredPosition + Vector2.up * floatHeight;
                
                _floatTween = _rectTransform
                    .DOAnchorPosY(targetPos.y, floatDuration)
                    .SetDelay(floatDelay)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetTarget(gameObject);
            }
        }

        /// <summary>
        /// Останавливает все анимации.
        /// </summary>
        public void StopAllAnimations()
        {
            _fadeTween?.Kill();
            _floatTween?.Kill();
            
            // Возвращаем исходные значения
            _canvasGroup.alpha = maxAlpha;
            
            if (useLocalPosition)
            {
                transform.localPosition = _originalLocalPosition;
            }
            else
            {
                _rectTransform.anchoredPosition = _originalAnchoredPosition;
            }
        }

        /// <summary>
        /// Останавливает анимацию исчезновения/появления.
        /// </summary>
        public void StopFadeAnimation()
        {
            _fadeTween?.Kill();
            _canvasGroup.alpha = maxAlpha;
        }

        /// <summary>
        /// Останавливает анимацию подпрыгивания.
        /// </summary>
        public void StopFloatAnimation()
        {
            _floatTween?.Kill();
            
            if (useLocalPosition)
            {
                transform.localPosition = _originalLocalPosition;
            }
            else
            {
                _rectTransform.anchoredPosition = _originalAnchoredPosition;
            }
        }

        private void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _floatTween?.Kill();
        }

        private void OnDisable()
        {
            // При отключении объекта останавливаем анимации
            StopAllAnimations();
        }

        private void OnEnable()
        {
            // При включении объекта перезапускаем анимации
            if (playOnStart)
            {
                PlayAllAnimations();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Предпросмотр анимации в редакторе.
        /// </summary>
        [ContextMenu("Preview Animations")]
        private void PreviewAnimations()
        {
            PlayAllAnimations();
        }

        /// <summary>
        /// Остановить предпросмотр в редакторе.
        /// </summary>
        [ContextMenu("Stop Preview")]
        private void StopPreview()
        {
            StopAllAnimations();
        }
#endif
    }
}
