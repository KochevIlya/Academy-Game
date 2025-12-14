using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Utils.Extensions;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls
{
  public class InputHelper : IInputHelper
  {
    private readonly ICameraService _cameraService;
    
    public InputHelper(ICameraService cameraService)
    {
      _cameraService = cameraService;
    }
    
    public bool ScreenToGroundPosition(Vector2 screenPosition, float groundHeight, out Vector3 worldPosition)
    {
      worldPosition = Vector3.zero;

      var plane = new Plane(Vector3.up, Vector3.zero.SetY(groundHeight));
      var ray = _cameraService.Camera.ScreenPointToRay(screenPosition);
      
      if (plane.Raycast(ray, out var distance))
      {
        worldPosition = ray.GetPoint(distance);
        return true;
      }

      return false;
    }
  }
}