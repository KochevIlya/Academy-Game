using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls.Variants;
using UnityEngine;
using Zenject;

public class InputControllsFactory
{
    [Inject] private ICameraService _cameraService;
    public IInputControls CreateControls(GameUnit unit)
    {
        return ChangeAggressiveControls(unit, null, null);
    }
    public IInputControls ChangeAggressiveControls(GameUnit unit, GameUnit target, HackingTerminal terminal)
    {
        var behaviourType = unit.Data.behaviourType;

        switch (behaviourType)
        {
                
            case UnitBehaviourType.Melee:
                return new WalkerInputControls(target, terminal);
                
            case UnitBehaviourType.Distance:
                return new AggroDistanceInputControls(unit, target);
            
            case UnitBehaviourType.Tank:
                return new AggroDistanceInputControls(unit, target);
            
            default:
                return new DummyInputControls();
        }
    }
    public IInputControls ChangeALlAggressiveControls(GameUnit unit, GameUnit target)
    {
        var behaviourType = unit.Data.behaviourType;

        switch (behaviourType)
        {
                
            case UnitBehaviourType.Melee:
                return new AggroMeleeInputControls(unit, target, _cameraService);
                
            case UnitBehaviourType.Distance:
                return new AggroDistanceInputControls(unit, target);
            
            default:
                return new DummyInputControls();
        }
    }
    public IInputControls ChangePatrollControls(GameUnit unit)
    {
        var behaviourType = unit.Data.behaviourType;
        switch (behaviourType)
        {
            default:
                return new PatrolInputControls(unit);
        }
    }
    
    
}
