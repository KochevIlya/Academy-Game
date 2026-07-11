using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit.Mover;
using UnityEngine;

public class DroneMover : MainCharacterMover
{
    protected override void ApplyMovement(Vector3 horizontal, float deltaTime)
    {
        _controller.Move(horizontal * deltaTime);
    }
}
