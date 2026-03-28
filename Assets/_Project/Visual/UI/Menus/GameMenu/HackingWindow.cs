using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HackingWindow : BaseScreen
{
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
    private List<Image> _spawnedArrows1 = new List<Image>();
    private IGuiGameService _guiGameService;
    [Inject]
    public void Construct(HackingService service
        ,IGuiGameService guiGameService
    )
    {
        _service = service;
        _guiGameService = guiGameService;
        
        Debug.Log($"HackingView заинжекчен. Сервис: {(_service != null ? "ОК" : "NULL")}");
    }
    
    private void Awake()
    {
        
         _service.OnHackingStarted
            .Subscribe(sequence => Show(sequence))
            .AddTo(this);

        _service.CurrentProgressIndex
            .Subscribe(index => UpdateProgress(index))
            .AddTo(this);

        _service.OnError
            .Subscribe(index => ShowError(index))
            .AddTo(this);

        UpdateProgress(0);
    }
    
    private void Show(List<Vector2> sequence)
    {
        Clear();

        foreach (var direction in sequence)
        {
            Image arrow = Instantiate(_arrowPrefab, _arrowsContainer);
            
            arrow.sprite = GetSprite(direction);
            arrow.color = _futureColor;
            _spawnedArrows1.Add(arrow);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_arrowsContainer);
        UpdateProgress(0);
    }

    private void UpdateProgress(int currentIndex)
    {
        for (int i = 0; i < _spawnedArrows1.Count; i++)
        {
            if (i < currentIndex)
                _spawnedArrows1[i].color = _completedColor;
            else if (i == currentIndex)
            {
                _spawnedArrows1[i].color = _currentColor;
                _spawnedArrows1[i].transform.localScale = Vector3.one * 1.3f;
            }
            else
            {
                _spawnedArrows1[i].color = _futureColor;
                _spawnedArrows1[i].transform.localScale = Vector3.one;
            }
        }
        
    }

    private void ShowError(int index)
    {
        if (index == -1) 
        {
            foreach (var arrow in _spawnedArrows1) 
            {
                arrow.color = _errorColor;
            }
            
        }
        else if (index == -2)
        {
            UpdateProgress(0); 
        }
    }

    private void Hide()
    {
        Clear();
    }
    
    private void Clear()
    {
        foreach (var arrow in _spawnedArrows1) 
            if(arrow != null) Destroy(arrow.gameObject);
        _spawnedArrows1.Clear();
    }
    
    private Sprite GetSprite(Vector2 direction)
    {
        if (direction == Vector2.up) return _upSprite;
        if (direction == Vector2.down) return _downSprite;
        if (direction == Vector2.left) return _leftSprite;
        return _rightSprite;
    }
    
    
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.HackingWindow;
    }
}
