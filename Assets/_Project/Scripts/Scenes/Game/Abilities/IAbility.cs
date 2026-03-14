using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using UniRx;
using UnityEngine;

public interface IAbility
{
    IReadOnlyReactiveProperty<bool> IsReady { get; }
    void Use(Vector3 targetPosition);
    bool CanUse();
    public void Initialize(GameUnit unit, AbilityConfig config);
    public BotAbilityType GetAbilityType();
}
