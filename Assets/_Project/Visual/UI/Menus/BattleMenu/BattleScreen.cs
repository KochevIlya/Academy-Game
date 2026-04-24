using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Visual.UI.Menus.BattleMenu
{
    public class BattleScreen : BaseScreen
    {
        // [SerializeField] private GameObject _ability;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Image _cooldownOverlay;
        [SerializeField] private TMP_Text _cooldownText;
        private IAbility _currentAbility;
        private Health _currentHealth;
        private IPlayerProvider _playerProvider;
        private CompositeDisposable _unitDisposables = new CompositeDisposable();
        
        [Inject]
        public void Construct(IPlayerProvider playerProvider)
        {
            _playerProvider = playerProvider;
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
            _currentHealth = newUnit.Health;
            
            if (ability == null)
            {
                Debug.LogWarning($"[BattleScreen] У юнита {newUnit.name} нет абилки!");
                return;
            }
            Debug.Log($"[BattleScreen] Подписываемся на абилку. Текущее состояние IsReady: {ability.IsReady.Value}");
            // ability.IsReady
            //     .Subscribe(ready => 
            //     {
            //         Debug.Log($"[BattleScreen] Реакция на изменение IsReady: {ready}");
            //         
            //         if(ready)
            //             _cooldownText.text = $"";
            //     })
            //     .AddTo(_unitDisposables);
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (ability.MaxCooldown > 0)
                    {
                        float fill = ability.CurrentTimer / ability.MaxCooldown;
                        if(!ability.IsReady.Value)
                            _cooldownText.text = $"{ability.CurrentTimer:F1}";
                        else
                        {
                            _cooldownText.text = $"";
                        }
                        _cooldownOverlay.fillAmount = fill;
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
