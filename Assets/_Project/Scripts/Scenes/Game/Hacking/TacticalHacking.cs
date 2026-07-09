using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Hacking;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class TacticalHacking : ITacticalHacking, IDisposable
{
    private const float _duration = 5f;
    
    private IHackingGame _hackingGame;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public ReactiveProperty<double> HackingUITimer { get; } = new ReactiveProperty<double>(0);
    public ReactiveProperty<bool> IsHacking { get; } = new ReactiveProperty<bool>(false);
    public ReactiveProperty<bool> CanHackProperty { get; } = new ReactiveProperty<bool>(false);
    public Subject<HackableComponent> OnHackingStarted { get; }


    private CombatZone _combatZone;
    private CancellationTokenSource _cts;
    private IPosessionService _posessionService;
    private HackableSelector _hackableSelector;
    private Transform _currentViewPoint;
    private ICameraService _cameraService;
    private List<HackableComponent> _hackableObjects;
    private ICursorService _cursorService;
    private IGuiGameService _guiGameService;
    
    private TimeSpan _time;

    [Inject]
    public void Construct(ArrowsMiniGame arrowsMiniGame
    ,IPosessionService posessionService
    ,HackableSelector hackableSelector
    ,ICameraService cameraService
    ,ICursorService cursorService
    ,IGuiGameService guiGameService
    )
    {
        _hackingGame = arrowsMiniGame;
        _posessionService = posessionService;
        _hackableSelector = hackableSelector;
        _cameraService = cameraService;
        _time = TimeSpan.Zero;
        _cursorService = cursorService;
        _guiGameService = guiGameService;
    }

    public void SetHackingZoneStatus(bool status)
    {
        CanHackProperty.Value = status;
    }

    public async void TryHack()
    {
        if (!CanHack()) return;
        
        _cts?.Cancel();
        _cts?.Dispose();
        
        _cts = new CancellationTokenSource();
        
        _time = TimeSpan.FromSeconds(_duration);
        
        _posessionService.UpdateBlocking(true);
        Debug.Log("Player Can Hack: Trying to Hack");
        IsHacking.Value = true;
        _cameraService.SetPoint(_currentViewPoint);
        
        
        _hackableSelector.SetAllowedTargets(_hackableObjects);
        Debug.Log($"[Tactical Hacking] num of el in _hackableObjects: {_hackableObjects.Count}");
        StartTimerCountdownAsync(_cts.Token).Forget();
        
        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                _guiGameService.ShowHackingSelectionWindow();
                HackableComponent selectedTarget = await _hackableSelector.SelectTarget(_cts.Token);
                
                await _guiGameService.CloseScreen(ScreenType.HackingSelectionWindow);
                
                if (selectedTarget == null)
                    return;
                
                await _guiGameService.ShowWindow(ScreenType.HackingWindow);
                
                await _hackingGame.StartHackingGame().ToUniTask(cancellationToken: _cts.Token);

                selectedTarget.Activate();
                
                    
                    
                
                await _guiGameService.CloseScreen(ScreenType.HackingWindow);
                _hackingGame.StopHackingGame();
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[TacticalHacking] Фаза выбора цели была прервана извне!");
            ResetHackingState();
            return;
        }
        finally
        {
            ResetHackingState();
        }
        
    }
    
    private async UniTaskVoid StartTimerCountdownAsync(CancellationToken ct)
    {
        while (_time.TotalSeconds > 0)
        {
            
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
            
            _time -= TimeSpan.FromSeconds(Time.deltaTime);

            HackingUITimer.Value = _time.TotalSeconds;
        }

        if (!ct.IsCancellationRequested)
        {
            Debug.Log("[Таймер] Время вышло! Нажимаем кнопку СТОП для всего взлома.");
            _cts?.Cancel(); 
            _combatZone.ActivateAggroAll();
              
        }
    }
    
    private void ResetHackingState()
    {
        Debug.Log("Генеральная уборка: сброс состояния взлома");
        IsHacking.Value = false;
        _posessionService.UpdateBlocking(false);
    
        _hackingGame.StopHackingGame(); 
    
        _cts?.Dispose();
        _cts = null;
        
        _posessionService.UpdateBlocking(false);
        IsHacking.Value = false;
        _cameraService.SetPoint(_posessionService.GetCurrentUnit().transform);
        
        _guiGameService.CloseScreen(ScreenType.HackingWindow);
        _guiGameService.CloseScreen(ScreenType.HackingSelectionWindow);
        _cursorService.SetInGameCursor();
    }
    
    public bool CanHack()
    {
        if (!CanHackProperty.Value
            || IsHacking.Value)
            return false;
        
        return true;
    }
    
    public void SetContext(Transform point, List<HackableComponent> units, CombatZone combatZone)
    {
        
        _combatZone =  combatZone;
        _hackableObjects = units;
        _currentViewPoint = point;
    }

    public void ClearContext()
    {
        _currentViewPoint = null;
    }

    public void CloseSelection()
    {
        _cts.Cancel();
    }

    public void Dispose()
    {
        HackingUITimer?.Dispose();
        IsHacking?.Dispose();
        CanHackProperty?.Dispose();
        _disposables?.Dispose();
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
