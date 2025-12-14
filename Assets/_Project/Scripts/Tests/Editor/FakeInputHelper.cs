using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using UnityEngine;

namespace _Project.Scripts.Tests.Editor
{
    public class FakeInputHelper : IInputHelper
    {
        public bool ShouldReturnTrue = true;
        public Vector3 WorldPositionToReturn = Vector3.zero;

        public bool ScreenToGroundPosition(Vector2 screenPos, float y, out Vector3 worldPos)
        {
            worldPos = WorldPositionToReturn;
            return ShouldReturnTrue;
        }
    }
}