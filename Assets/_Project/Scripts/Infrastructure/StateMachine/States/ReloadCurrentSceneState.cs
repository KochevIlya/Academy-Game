using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class ReloadCurrentSceneState : IEnterState
{
    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
        Time.timeScale = 1f;
        string sceneName = SceneManager.GetActiveScene().name;

        await SceneManager.LoadSceneAsync(sceneName);
        Resources.UnloadUnusedAssets();
        await UniTask.Yield();

        gameStateMachine.Enter<SpawnGameState>();
    }
}
