using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ReloadCurrentSceneState : IEnterState
{
    private readonly ZenjectSceneLoader _sceneLoader;

    public ReloadCurrentSceneState(ZenjectSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {

        string currentSceneName = SceneManager.GetActiveScene().name;

        await _sceneLoader.LoadSceneAsync(currentSceneName, LoadSceneMode.Single);

        gameStateMachine.Enter<LoadProjectState>();
        Debug.Log("After Reloading");
    }
}