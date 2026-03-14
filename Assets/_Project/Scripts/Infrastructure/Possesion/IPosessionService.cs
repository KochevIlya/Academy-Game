using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

public interface IPosessionService
{
    public void Possess(GameUnit newUnit);
    
    public void UpdateBlocking(bool blocking);

}
