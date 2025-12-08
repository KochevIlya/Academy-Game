using System;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit
{
    public interface IUnitRotator
    {
        void Rotate(GameUnit gameUnit, Vector2 movementDelta);
    }
}