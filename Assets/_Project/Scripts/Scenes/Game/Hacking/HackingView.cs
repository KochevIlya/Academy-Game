using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HackingView : MonoBehaviour
{ 
    [SerializeField] private GameObject _rootVisuals;
    [SerializeField] private RectTransform _arrowsContainer;
    [SerializeField] private Image _arrowPrefab;
        
    [SerializeField] private Sprite _upSprite;
    [SerializeField] private Sprite _downSprite;
    [SerializeField] private Sprite _leftSprite;
    [SerializeField] private Sprite _rightSprite;

    [SerializeField] private Color _completedColor = Color.green;
    [SerializeField] private Color _currentColor = Color.white;
    [SerializeField] private Color _futureColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color _errorColor = Color.red;

    private HackingService _service;
    private List<Image> _spawnedArrows = new List<Image>();
    
    [Inject]
    public void Construct(HackingService service)
    {
        _service = service;
        
        Debug.Log($"HackingView заинжекчен. Сервис: {(_service != null ? "ОК" : "NULL")}");
    }
    
    private void Start()
    {
        Debug.Log($"[Check View] Service: {_service != null}, Root: {_rootVisuals != null}, Container: {_arrowsContainer != null}");
        _rootVisuals.SetActive(false);
        
         _service.OnHackingStarted
            .Subscribe(sequence => Show(sequence))
            .AddTo(this);

        _service.CurrentProgressIndex
            .Subscribe(index => UpdateProgress(index))
            .AddTo(this);

        _service.OnError
            .Subscribe(index => ShowError(index))
            .AddTo(this);

        _service.IsHacking
            .Where(isHacking => !isHacking)
            .Subscribe(_ => Hide())
            .AddTo(this);
    }

    private void Show(List<Vector2> sequence)
    {
        Debug.Log("HackingView: Получен сигнал на запуск визуализации!");
        _rootVisuals.SetActive(true);
        Clear();

        foreach (var direction in sequence)
        {
            Image arrow = Instantiate(_arrowPrefab, _arrowsContainer);
            arrow.sprite = GetSprite(direction);
            arrow.color = _futureColor;
            _spawnedArrows.Add(arrow);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_arrowsContainer);
    }

    private void UpdateProgress(int currentIndex)
    {
        for (int i = 0; i < _spawnedArrows.Count; i++)
        {
            if (i < currentIndex)
                _spawnedArrows[i].color = _completedColor;
            else if (i == currentIndex)
            {
                _spawnedArrows[i].color = _currentColor;
                _spawnedArrows[i].transform.localScale = Vector3.one * 1.3f;
            }
            else
            {
                _spawnedArrows[i].color = _futureColor;
                _spawnedArrows[i].transform.localScale = Vector3.one;
            }
        }
    }

    private void ShowError(int index)
    {
        if (index < _spawnedArrows.Count)
            _spawnedArrows[index].color = _errorColor;
    }

    private void Hide()
    {
        _rootVisuals.SetActive(false);
        Clear();
    }
    
    private void Clear()
    {
        foreach (var arrow in _spawnedArrows) 
            if(arrow != null) Destroy(arrow.gameObject);
        _spawnedArrows.Clear();
    }
    
    private Sprite GetSprite(Vector2 direction)
    {
        if (direction == Vector2.up) return _upSprite;
        if (direction == Vector2.down) return _downSprite;
        if (direction == Vector2.left) return _leftSprite;
        return _rightSprite;
    }
    
}