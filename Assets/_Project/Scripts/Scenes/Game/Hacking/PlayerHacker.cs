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

    private ITacticalHacking _tacticalHacking;
    private UserInputControls _input;
    private GameUnit _myUnit;
    private IPosessionService _posessionService;
    [Inject] HackableSelector _hackableSelector; 
    
    [Inject]
    public void Construct(ITacticalHacking hackingService
        , UserInputControls input
    , IPosessionService posessionService
    
    )
    { 
        _tacticalHacking = hackingService;
        _input = input;
        posessionService.Possess(GetComponent<GameUnit>());
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
            _input.OnAction
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
        Debug.Log("PlayerHacker: TryToHack, isPosessing: ");
        
        _tacticalHacking.TryHack();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
}
