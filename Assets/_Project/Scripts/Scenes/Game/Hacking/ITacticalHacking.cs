using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UniRx;
using UnityEngine;

public interface ITacticalHacking : IDisposable
{
    public ReactiveProperty<double> HackingUITimer { get; }
    public ReactiveProperty<bool> IsHacking { get; }
    public ReactiveProperty<bool> CanHackProperty { get; }
    public Subject<HackableComponent> OnHackingStarted { get; }
    public void SetContext(Transform point, List<HackableComponent> objects, CombatZone combatZone);
    public void ClearContext();
    
    public void SetHackingZoneStatus(bool status);
    public void TryHack();
}
