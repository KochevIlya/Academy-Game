using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public abstract class BaseAbility<T> : MonoBehaviour, IAbility where T : AbilitySettings
{
    [Inject] protected readonly UserInputControls _input;
    [Inject] protected readonly IInputHelper _inputHelper;
    
    protected readonly ReactiveProperty<bool> _isReady = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IsReady => _isReady;
    protected AbilityConfig _abilityConfig;
    protected T Settings => _settings as T;
    protected AbilitySettings _settings;
    protected float _timer;
    
    protected GameUnit _unit;
    
    public virtual void Use(Vector3 targetPosition)
    {
        if (!CanUse()) return;
        _isReady.Value = false;   
        UseAbility().Forget();
        _timer = _settings.cooldown;
        
    }

    protected abstract UniTask UseAbility();
    

    public virtual bool CanUse() => _settings != null && _timer <= 0;

    public virtual void Initialize(GameUnit unit, AbilityConfig config)
    {
        _unit = unit;
        _abilityConfig = config;
        _isReady.Value = true;
    }

    public abstract BotAbilityType GetAbilityType();
    
    protected void OnDestroy()
    {
        _isReady.Dispose();
    }
    protected void Update()
    {
        if (_settings != null && _timer > 0)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0 && !_isReady.Value)
            {
                _isReady.Value = true;
            }
        }
    }
    
    
}
