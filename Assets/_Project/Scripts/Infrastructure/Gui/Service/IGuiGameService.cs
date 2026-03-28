using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace _Project.Scripts.Infrastructure.Gui.Service
{
    public interface IGuiGameService
    {
        Canvas.StaticCanvas StaticCanvas { get; }
        void Push(BaseScreen screen);
        void Pop();
        UniTask Cleanup();
        void ShowGameOver();
        void ShowPauseMenuWindow();
        void ShowBattleScreen();
        void ShowPauseButton();
        void ShowControlsWindow();
        void ShowMainMenuWindow(bool isAlreadySaved = true);
        void ShowSaveMenuWindow();
        void ShowHackingSelectionWindow();
        UniTask<BaseScreen> ShowWindow(ScreenType screenType);
        UniTask CloseScreen(BaseScreen screen);
        UniTask CloseScreen(ScreenType screenType);
        public UniTask ShowHackingWindow();
    }
}
