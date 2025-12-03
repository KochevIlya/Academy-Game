using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Gui.Screens
{
  public abstract class BaseScreen : MonoBehaviour
  {
    [SerializeField] private CanvasGroup _canvasGroup;
        
    public readonly ReactiveCommand CloseScreen = new ();
        
    protected readonly CompositeDisposable LifeTimeDisposable = new();

    protected virtual void OnEnable() { }
    protected virtual void OnDisable() => LifeTimeDisposable.Clear();
    public virtual void Localize() { }

    public abstract ScreenType GetScreenType();

    public virtual async UniTask Show()
    {
      SetCanvasEnable(false);
      await FadeCanvas(0f, 1f).AsyncWaitForCompletion().AsUniTask();
      SetCanvasEnable(true);
    }
        
    protected virtual async UniTask Hide()
    {
      SetCanvasEnable(false);
    }
        
    public void SetActive(bool isActive)
    {
      _canvasGroup.alpha = isActive ? 1f : 0f;
      _canvasGroup.interactable = isActive;
      _canvasGroup.blocksRaycasts = isActive;
    }

    protected Tween FadeCanvas(float from, float to)
    {
      return _canvasGroup
        .DOFade(to, 0.15f)
        .From(from)
        .SetEase(Ease.Linear)
        .SetLink(gameObject);
    }

    private void SetCanvasEnable(bool isEnable)
    {
      _canvasGroup.interactable = isEnable;
      _canvasGroup.blocksRaycasts = isEnable;
    }
  }
  
  public abstract class BaseScreen<T> : BaseScreen
  {
    public new readonly ReactiveCommand<T> CloseScreen = new ();
    
    protected virtual async UniTask Hide(T data)
    {
      await base.Hide();
    }
  }
}