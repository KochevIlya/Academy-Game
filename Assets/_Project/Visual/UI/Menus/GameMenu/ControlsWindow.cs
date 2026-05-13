using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ControlsWindow : BaseScreen
{
    public override bool IsOverlay => true;
    [SerializeField] protected Button _exitButton;
    [SerializeField] protected Slider _slider;
    protected IGuiService _guiService;
    protected ISoundService _soundService;
    
    [Inject]
    public virtual void Construct(
        IGuiService guiService
    ,ISoundService soundService
        )
    {
        _guiService = guiService;
        _soundService = soundService;
    }
    protected virtual void Awake() 
    {
        _exitButton.onClick.AddListener(BackToMenu);
        
    }
    
    protected void Start()
    {
        _slider.value = _soundService.Volume;
        
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    protected void OnSliderValueChanged(float newValue)
    {
        
        _soundService.Volume = newValue;
    }
    
    
    protected virtual void BackToMenu()
    {
        _guiService.Pop();
    }
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.ControlsWindow;
    }
    private void OnDestroy()
    {
        if (_slider != null)
        {
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}
