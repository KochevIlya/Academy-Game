using _Project.Scripts.Infrastructure.PersistentProgress.Data;

namespace _Project.Scripts.Infrastructure.PersistentProgress
{
  public interface ISavedProgress : ISavedProgressReader
  {
    void UpdateProgress(PlayerProgress progress);
  }
}