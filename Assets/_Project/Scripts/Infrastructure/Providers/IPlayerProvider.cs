using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UniRx;
using UnityEngine;

public interface IPlayerProvider 
{
    IReadOnlyReactiveProperty<GameUnit> ActiveUnit { get; }
    void SetActiveUnit(GameUnit unit);
}
