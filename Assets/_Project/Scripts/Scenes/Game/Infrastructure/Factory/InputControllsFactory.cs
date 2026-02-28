using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UnityEngine;

public class InputControllsFactory
{
    public IInputControls CreateControls(GameUnit unit)
    {
        return ChangeAggressiveControls(unit, null);
    }
    public IInputControls ChangeAggressiveControls(GameUnit unit, GameUnit target)
    {
        var behaviourType = unit.Data.behaviourType;

        switch (behaviourType)
        {
                
            case UnitBehaviourType.Melee:
                return new AggroMeleeInputControls(unit, target);
                
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
