using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.AssetProvider;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StaticData;
using _Project.Scripts.Libs.Configs.Loader;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Sounds;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Installers
{
  [Serializable]
  public struct AudioData
  { 
    public Audio.AudioType Type;
    public AudioSource Source;
  }
  
public class ProjectServicesInstaller : MonoInstaller
  {
    
    [SerializeField] private GuiService _guiServicePrefab;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _warSoundAudioSource;
    
    [SerializeField] private List<AudioData> _audioSources;
    [SerializeField] private List<AudioData> _audioEffectsSources;
    
    public override void InstallBindings()
    {
      Container
        .BindInterfacesTo<GuiService>()
        .FromComponentInNewPrefab(_guiServicePrefab)
        .AsSingle()
        .NonLazy();
      
      Container.Bind<AudioSource>()
        .WithId("Global")
        .FromInstance(_musicAudioSource);
      
      Container.Bind<AudioSource>()
        .WithId("War")
        .FromInstance(_warSoundAudioSource);
      
      var audioDictionary = new Dictionary<Audio.AudioType, AudioSource>();
      foreach (var audio in _audioSources)
      {
        if (audio.Source != null && !audioDictionary.ContainsKey(audio.Type))
        {
          audioDictionary.Add(audio.Type, audio.Source);
        }
      }

      var effectsDictionary = new Dictionary<Audio.AudioType, AudioSource>();
      foreach (var audio in _audioEffectsSources)
      {
        if (audio.Source != null && !effectsDictionary.ContainsKey(audio.Type))
        {
          effectsDictionary.Add(audio.Type, audio.Source);
        }
      }


      Container.Bind<IReadOnlyDictionary<Audio.AudioType, AudioSource>>()
        .WithId("Global")
        .FromInstance(audioDictionary);
      
      Container.Bind<IReadOnlyDictionary<Audio.AudioType, AudioSource>>()
        .WithId("Effects")
        .FromInstance(effectsDictionary);
      
      Container.Bind<ISoundService>().To<SoundService>().AsSingle().NonLazy();
      Container.Bind<IProgressService>().To<ProgressService>().AsSingle();
      Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
      Container.Bind<IConfigsLoader>().To<ConfigsLoader>().AsSingle();
      Container.Bind<IAssetProvider>().To<AssetProvider.AssetProvider>().AsSingle();
      Container.Bind<ICursorService>().To<CursorService>().AsSingle();
      Container.Bind<SceneLoaderService>().AsSingle();
      
      
      
      Container.Bind<IMenuActionsService>().To<MenuActionsService>().AsSingle();
      
      
    }
  }
}