using System;
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
using Object = UnityEngine.Object;

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
        Debug.Log("PlayerHacker: Start");
        try
        {
            _input.OnHacking
                .Where(_ => !_hackingService.IsHacking.Value)
                .TakeUntilDestroy(this)
                .Subscribe(_ => TryToHack())
                .AddTo(this);
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private async void TryToHack()
    {
        if (!(_myUnit.InputControls is UserInputControls)) return;
        Debug.Log("PlayerHacker: TryToHack, isPosessing: " + _hackingService.IsPossessing);
        if (_hackingService.IsPossessing) return;
        _hackingService.RequestHacking(_myUnit);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
}
