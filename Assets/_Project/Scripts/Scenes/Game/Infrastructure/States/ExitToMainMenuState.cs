using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMainMenuState : IEnterState
{

    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
        Time.timeScale = 0f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        await SceneManager.LoadSceneAsync(currentSceneName).ToUniTask();

        gameStateMachine.Enter<MainMenuState>();
    }
}
