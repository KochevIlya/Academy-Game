using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.PersistentProgress.Data;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.SaveLoad
{
  public interface ISaveLoadService
  {
    public void Save();
    public UniTask LoadAsync();
    void RegisterUnit(IUnitSaveable saveable);
    void UnregisterUnit(IUnitSaveable saveable);
    void RegisterTerminal(ITerminalSaveable saveable);
    void UnregisterTerminal(ITerminalSaveable saveable);
  }
}