using _Project.Scripts.Infrastructure.PersistentProgress.Data;

namespace _Project.Scripts.Infrastructure.PersistentProgress
{
  public interface IPersistentProgressService
  {
    PlayerProgress Progress { get; set; }
  }
}