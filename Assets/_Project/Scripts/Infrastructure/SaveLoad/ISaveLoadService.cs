using _Project.Scripts.Infrastructure.PersistentProgress.Data;

namespace _Project.Scripts.Infrastructure.SaveLoad
{
  public interface ISaveLoadService
  {
    void SaveProgress();
    PlayerProgress LoadProgress();
  }
}