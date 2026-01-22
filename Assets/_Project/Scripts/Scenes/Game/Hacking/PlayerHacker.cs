using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Scenes.Game.Hacking;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
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
    [Inject] HackableSelector _hackableSelector; 
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

    private async void TryToHack()
    {
        if (!(_myUnit.InputControls is UserInputControls)) return;
        if (_hackingService.IsPossessing) return;
        _hackingService.RequestHacking(_myUnit);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
    private HackableComponent FindClosestTarget()
    {
        var allTargets = Object.FindObjectsByType<HackableComponent>(FindObjectsSortMode.None);

        return allTargets
            .Where(t => t.gameObject != this.gameObject)
            .Select(t => new 
            { 
                Component = t, 
                Distance = Vector3.Distance(transform.position, t.transform.position) 
            })
            .Where(t => t.Distance <= _interactionRadius)
            .OrderBy(t => t.Distance)
            .Select(t => t.Component)
            .FirstOrDefault();
    }
}
