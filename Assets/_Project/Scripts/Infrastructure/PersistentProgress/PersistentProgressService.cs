using _Project.Scripts.Infrastructure.PersistentProgress.Data;

namespace _Project.Scripts.Infrastructure.PersistentProgress
{
  public class PersistentProgressService : IPersistentProgressService
  {
    public PlayerProgress Progress { get; set; }
  }
}