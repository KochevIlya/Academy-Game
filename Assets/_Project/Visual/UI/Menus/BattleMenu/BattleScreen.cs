using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Scenes.Game.Unit;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Visual.UI.Menus.BattleMenu
{
    public class BattleScreen : BaseScreen
    {
        [SerializeField] private Button _abilityButton;
        private IAbility _currentAbility;
        private IPlayerProvider _playerProvider;
        private CompositeDisposable _unitDisposables = new CompositeDisposable();
        public override bool IsOverlay => true;
        
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
            return  ScreenType.Battle;
        }
        
        private void OnUnitChanged(GameUnit newUnit)
        {
            _unitDisposables.Clear();

            if (newUnit == null) return;
            
            var ability = newUnit.Ability; 
        
            if (ability == null)
            {
                Debug.LogError($"[BattleScreen] У юнита {newUnit.name} нет абилки!");
                return;
            }
            Debug.Log($"[BattleScreen] Подписываемся на абилку. Текущее состояние IsReady: {ability.IsReady.Value}");
            ability.IsReady
                .Subscribe(ready => 
                {
                    Debug.Log($"[BattleScreen] Реакция на изменение IsReady: {ready}");
                    ShowAbilityButton(ready);
                })
                .AddTo(_unitDisposables);
        }
        
        public void ShowAbilityButton(bool isActive)
        {
            _abilityButton.gameObject.SetActive(isActive);
        }
        
    }
}
