using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UniRx;
using UnityEngine;

public class PlayerProvider : IPlayerProvider
{
    private readonly ReactiveProperty<GameUnit> _activeUnit = new ReactiveProperty<GameUnit>();
    
    public IReadOnlyReactiveProperty<GameUnit> ActiveUnit => _activeUnit;

    public void SetActiveUnit(GameUnit unit)
    {
        _activeUnit.Value = unit;
    }
}
