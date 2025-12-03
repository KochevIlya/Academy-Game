using System;
using System.Numerics;
using UniRx;

namespace _Project.Scripts.Scenes.Game.Unit.Controls
{
  public class UserInputControls : IInputControls
  {
    private readonly Subject<Vector2> _movement = new();
    private readonly Subject<Vector2> _rotation = new();
    
    public IObservable<Vector2> OnMovement => _movement;
    public IObservable<Vector2> OnRotation => _rotation;


    //_movement.OnNext(directionDelta);
    //_rotation.OnNext(rotationDelta);
  }
}