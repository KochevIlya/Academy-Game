using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Visual.UI.Menus.GameMenu
{
    public class AbilitiesTutorialWindow : BaseScreen
    {
        private HackingService _hackingService;
        private IGuiGameService _guiService;
        private IPlayerProvider _playerProvider;
        private readonly SerialDisposable _abilitySubscription = new SerialDisposable();
        [SerializeField] private SkipPanel _skipHandler;
    
        [Inject]
        public void Construct(
            HackingService hackingService
            ,IGuiGameService guiService
            ,IPlayerProvider playerProvider
        )
        {
            _hackingService = hackingService;
            _guiService = guiService;
            _playerProvider = playerProvider;
        }
        
        public override async UniTask Show()
        {
            if (_hackingService != null && _hackingService.IsBattleActive.Value)
            {
                gameObject.SetActive(true);
                Time.timeScale = 0f;
                await base.Show();
            }
        }
        private void Awake()
        {
            SetActive(false); 
        }
        private void Start()
        {
            _hackingService.IsBattleActive
                .Subscribe(isActive => SwitchWindow(isActive))
                .AddTo(LifeTimeDisposable);
            
            _playerProvider.ActiveUnit
                .Subscribe(OnUnitChanged)
                .AddTo(LifeTimeDisposable);
            
            if (_skipHandler != null)
            {
                _skipHandler.OnSkipPressed
                    .Subscribe(_ => CloseTutorial())
                    .AddTo(LifeTimeDisposable);
            }
        }
        private void OnUnitChanged(GameUnit unit)
        {
            if (unit?.Ability == null) return;

            _abilitySubscription.Disposable = unit.Ability.OnUsed
                .Subscribe(_ => CloseTutorial());
        }
        private void CloseTutorial()
        {
            Time.timeScale = 1f;
            Debug.Log("[Abilities Tutorial] Ability used! Closing...");
            _guiService.CloseScreen(GetScreenType()).Forget(); 
        }
        private void SwitchWindow(bool isActive)
        {
            Time.timeScale = 1f;
            if (isActive)
            {
                Debug.Log($"[Abilities Tutorial Window] Battle active. Showing window.");
                
                Show().Forget();
            }
            else
            {
                DG.Tweening.DOTween.Kill(gameObject);
                SetActive(false); 
            }
        }
    
        public override ScreenType GetScreenType()
        {
            return ScreenType.AbilitiesTutorialWindow;
        }
    }
}
