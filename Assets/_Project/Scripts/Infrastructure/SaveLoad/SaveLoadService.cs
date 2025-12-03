using _Project.Scripts.Infrastructure.PersistentProgress;
using _Project.Scripts.Infrastructure.PersistentProgress.Data;
using _Project.Scripts.Utils.Extensions;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.SaveLoad
{
  public class SaveLoadService : ISaveLoadService
  {
    private const string ProgressKey = "Progress";

    private readonly IPersistentProgressService _progressService;

    public SaveLoadService(IPersistentProgressService progressService)
    {
      _progressService = progressService;
    }

    public void SaveProgress()
    {
      PlayerPrefs.SetString(ProgressKey, _progressService.Progress.ToJson());
    }

    public PlayerProgress LoadProgress() => PlayerPrefs.GetString(ProgressKey)?
      .ToDeserialized<PlayerProgress>();
  }
}