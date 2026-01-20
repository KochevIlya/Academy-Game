using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UniRx;
using UnityEngine;
using Zenject;

public class PlayerHacker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _interactionRadius = 10f;

    private HackingService _hackingService;
    private UserInputControls _input;
    private GameUnit _myUnit;

    [Inject]
    public void Construct(HackingService hackingService, UserInputControls input)
    {
        _hackingService = hackingService;
        _input = input;
    }

    private void Awake()
    {
        _myUnit = GetComponent<GameUnit>();
    }

    private void Start()
    {
        _input.OnHacking
            .Where(_ => !_hackingService.IsHacking.Value)
            .Subscribe(_ => TryToHack())
            .AddTo(this);
    }

    private void TryToHack()
    {
        if (!(_myUnit.InputControls is UserInputControls)) 
        {
            return;
        }
        if (_hackingService.IsPossessing) 
        {
            Debug.Log("В этом теле нельзя использовать взлом (вы уже вселились)!");
            return;
        }
        var allTargets = Object.FindObjectsByType<HackableComponent>(FindObjectsSortMode.None);

        var bestTarget = allTargets
            .Where(t => t.gameObject != this.gameObject)
            .Select(t => new { Component = t, Distance = Vector3.Distance(transform.position, t.transform.position) })
            .Where(t => t.Distance <= _interactionRadius)
            .OrderBy(t => t.Distance)
            .Select(t => t.Component)
            .FirstOrDefault();
        
        if (bestTarget != null)
        {
            Debug.Log($"[TestHacking] Цель найдена: {bestTarget.name}. Дистанция: {Vector3.Distance(transform.position, bestTarget.transform.position)}");
            _hackingService.StartHacking(bestTarget, _myUnit);
        }
        else
        {
            Debug.LogWarning("[TestHacking] Рядом нет подходящих целей для взлома!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
}
