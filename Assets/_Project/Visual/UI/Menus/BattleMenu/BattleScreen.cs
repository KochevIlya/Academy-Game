using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Project.Visual.UI.Menus.BattleMenu
{
    [Serializable]
    public struct AbilityPair
    {
        public GameObject PrefabOn;
        public GameObject PrefabOff;
        public Image CooldownOverlay;
        public TMP_Text CooldownText;
    }
    [Serializable]
    public struct AbilityPrefabMapping
    {
        [FormerlySerializedAs("AbilityType")] public BotAbilityType BotAbilityType; 
        public AbilityPair Prefab;
    }
    public class BattleScreen : BaseScreen
    {
        
        
        // [SerializeField] private GameObject _ability;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Image _cooldownOverlay;
        [SerializeField] private TMP_Text _cooldownText;
        [FormerlySerializedAs("_screenMappings")] [SerializeField] private List<AbilityPrefabMapping> _abilityMappings;
        private Dictionary<BotAbilityType, AbilityPair> _prefabsDictionary;
        private IAbility _currentAbility;
        private Health _currentHealth;
        private IPlayerProvider _playerProvider;
        private CompositeDisposable _unitDisposables = new CompositeDisposable();
        private AbilityPair _abilityPair;
        
        [Inject]
        public void Construct(IPlayerProvider playerProvider)
        {
            _playerProvider = playerProvider;
            _prefabsDictionary = new Dictionary<BotAbilityType, AbilityPair>();
            foreach (var mapping in _abilityMappings)
            {
                if (!_prefabsDictionary.ContainsKey(mapping.BotAbilityType))
                {
                    _prefabsDictionary.Add(mapping.BotAbilityType, mapping.Prefab);
                }
            }

        }
        
        private void Start()
        {
            _playerProvider.ActiveUnit
                .Subscribe(OnUnitChanged)
                .AddTo(this);
        }
        
        public override ScreenType GetScreenType()
        {
            return ScreenType.Battle;
        }
        
        private void OnUnitChanged(GameUnit newUnit)
        {
            _unitDisposables.Clear();
            
            if (newUnit == null) return;
            
            var ability = newUnit.Ability;
            if (ability == null)
                return;
            AbilityPair pair = new AbilityPair();
            if (_prefabsDictionary != null && _prefabsDictionary.TryGetValue(ability.GetAbilityType(), out var mappedPair))
            {
                pair = mappedPair;
            }
            
            
            _currentHealth = newUnit.Health;
            //
            // _abilityPair.PrefabOn.SetActive(true);
            
            Debug.Log($"[BattleScreen] Current _abilityPair {_abilityPair}, PrefabOn {_abilityPair.PrefabOn}, PrefabOff {_abilityPair.PrefabOff}");
            Debug.Log($"[BattleScreen] Подписываемся на абилку. Текущее состояние IsReady: {ability.IsReady.Value}");
            
            // ability.IsReady
            //     .Subscribe(ready => 
            //     {
            //         Debug.Log($"[BattleScreen] Реакция на изменение IsReady: {ready}");
            //         if (ready)
            //         {
            //             _cooldownText.text = $"";
            //             prefab.PrefabOn.SetActive(true);
            //             prefab.PrefabOff.SetActive(false);
            //         }
            //         else
            //         {
            //             prefab.PrefabOn.SetActive(true);
            //             prefab.PrefabOff.SetActive(false);
            //         }
            //             
            //     })
            //     .AddTo(_unitDisposables);
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (ability.MaxCooldown > 0)
                    {
                        float fill = ability.CurrentTimer / ability.MaxCooldown;
                        if (!ability.IsReady.Value)
                        {
                            
                            if(pair.CooldownOverlay != null)
                                pair.CooldownText.text = $"{ability.CurrentTimer:F1}";
                            if(pair.PrefabOn != null)
                                pair.PrefabOn.SetActive(false);
                            if(pair.PrefabOff != null)
                                pair.PrefabOff.SetActive(true);
                        }
                        else
                        {
                            if(pair.CooldownOverlay != null)
                                pair.CooldownText.text = $"";
                            if(pair.PrefabOn != null)
                                pair.PrefabOn.SetActive(true);
                            if(pair.PrefabOff != null)
                                pair.PrefabOff.SetActive(false);
                        }
                        pair.CooldownOverlay.fillAmount = fill;
                    }
                })
                .AddTo(_unitDisposables);
            _currentHealth.CurrentHealth.Subscribe(_ =>
                {
                    ChangeHealth();
                })
            .AddTo(_unitDisposables);
            
        }
        
        private void ShowAbilityButton(bool isActive)
        {
            // _ability.SetActive(!isActive);
        }

        private void ChangeHealth()
        {
            _healthSlider.value = (float)_currentHealth.CurrentHealth.Value / _currentHealth.MaxHealth.Value;
        }
        
    }
}
