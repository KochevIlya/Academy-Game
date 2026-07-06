using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class HackingSelectionWindow : BaseScreen
{
    
    [SerializeField] private TextMeshProUGUI _textView;
    private TimeSpan _time;
    private ITacticalHacking _tacticalHacking;
    
    [Inject]
    public void Construct(
        ITacticalHacking tacticalHacking
    )
    {
        _tacticalHacking = tacticalHacking;
    }
    private void Awake()
    { 
        
        

        _tacticalHacking.HackingUITimer
            .Subscribe(time =>
            {
                _time = TimeSpan.FromSeconds(time);
                string seconds = _time.Seconds > 10 ? _time.Seconds.ToString() : $"0{_time.Seconds}";
                _textView.text = $"{_time.Minutes}:{seconds}";
            })
            .AddTo(this);

    }
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.HackingSelectionWindow;
    }
}
