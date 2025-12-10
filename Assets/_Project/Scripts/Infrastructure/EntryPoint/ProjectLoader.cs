using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.EntryPoint
{
  public class ProjectLoader
  {
    [RuntimeInitializeOnLoadMethod]
    public static void InitializeProjectContext()
    {
      StartProjectContext(ProjectContext.Instance);
    }
    
    private static void StartProjectContext(ProjectContext instance) { }
  }
}