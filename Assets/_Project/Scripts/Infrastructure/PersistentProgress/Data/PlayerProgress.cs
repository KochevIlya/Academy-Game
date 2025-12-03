using System;

namespace _Project.Scripts.Infrastructure.PersistentProgress.Data
{
  [Serializable]
  public class PlayerProgress
  {
    public float BestScore;
    public bool TutorialCompleted;
    
    public PlayerProgress()
    {
      
    }
  }
}