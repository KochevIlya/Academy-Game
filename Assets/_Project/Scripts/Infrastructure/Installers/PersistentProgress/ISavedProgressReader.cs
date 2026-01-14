using _Project.Scripts.Infrastructure.PersistentProgress.Data;

namespace _Project.Scripts.Infrastructure.PersistentProgress
{
  public interface ISavedProgressReader
  {
    void LoadProgress(PlayerProgress progress);
  }
}