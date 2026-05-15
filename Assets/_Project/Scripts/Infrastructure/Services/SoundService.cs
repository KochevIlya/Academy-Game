using System.Collections;
using System.Collections.Generic;
using _Project.Sounds;
using UnityEngine;
using Zenject;

public class SoundService : ISoundService
{
    private readonly AudioSource _globalAudioSource;
    private readonly AudioSource _warAudioSource;
    
    private readonly IReadOnlyDictionary<Audio.AudioType, AudioSource> _audioDict;
    private readonly IReadOnlyDictionary<Audio.AudioType, AudioSource> _effectsDict;
    
    private float _volume;
    private float _effectsVolume;
    
    public SoundService(
        [Inject(Id = "Global")]AudioSource globalAudioSource
        ,[Inject(Id = "War")]AudioSource warAudioSource
        ,[Inject (Id = "Global")]IReadOnlyDictionary<Audio.AudioType, AudioSource> audioDict
        ,[Inject (Id = "Effects")]IReadOnlyDictionary<Audio.AudioType, AudioSource> effectsDict
        )
        
    {
        _audioDict = audioDict;
        _effectsDict = effectsDict;
        
        _globalAudioSource = globalAudioSource;
        _warAudioSource = warAudioSource;
        _volume = globalAudioSource.volume;
        _effectsVolume = globalAudioSource.volume;
        PlayGlobalFromBeginning();
        StopWar();
        if (_globalAudioSource != null)
        {
            _globalAudioSource.volume = _volume;
            _warAudioSource.volume = _volume;
        }
    }
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Mathf.Clamp01(value); 
            
            foreach (var source in _audioDict.Values)
            {
                source.volume = _volume;
            }
        }
    }

    public float EffectVolume
    {
        get => _effectsVolume;
        set
        {
            _effectsVolume = Mathf.Clamp01(value);
            foreach (var source in _effectsDict.Values)
            {
                source.volume = _effectsVolume;
            }
        }
    }

    public void PlayGlobal()
    {
        if (_globalAudioSource != null && !_globalAudioSource.isPlaying)
        {
            _globalAudioSource.UnPause();
            if (!_globalAudioSource.isPlaying) 
                _globalAudioSource.Play();
        }
    }

    public void StopGlobal()
    {
        if (_globalAudioSource != null && _globalAudioSource.isPlaying)
        {
            _globalAudioSource.Pause();
        }
    }

    public void PlayGlobalFromBeginning()
    {
        if (_globalAudioSource != null && !_globalAudioSource.isPlaying)
        {
            _globalAudioSource.Play();
        }
    }

    public void PlayWar()
    {
        if (_warAudioSource != null && !_warAudioSource.isPlaying)
        {
            Debug.Log("[Sound Service] Play War]");
            _warAudioSource.Play();
        }
    }

    public void StopWar()
    {
        if (_warAudioSource != null && _warAudioSource.isPlaying)
        {
            _warAudioSource.Stop();
        }
    }

    public void PlayWarFromBeginning()
    {
        if (_warAudioSource != null && !_warAudioSource.isPlaying)
        {
            _warAudioSource.Play();
        }
    }

    public void Play(Audio.AudioType type, bool fromBeginning = false)
    {
        if (TryGetSource(type, out var source))
        {
            if (fromBeginning)
            {
                source.Stop();
                source.Play();
            }
            else if (!source.isPlaying)
            {
                source.Play();
            }
        }
    }
    

    private bool TryGetSource(Audio.AudioType type, out AudioSource source)
    {
        if (_audioDict.TryGetValue(type, out source) && source != null)
        {
            return true;
        }
        
        if (_effectsDict.TryGetValue(type, out source) && source != null)
        {
            return true;
        }
        
        Debug.LogWarning($"[Sound Service] AudioSource for type {type} not found or null!");
        return false;
    }
    public void Stop(Audio.AudioType type)
    {
        if (TryGetSource(type, out var source) && source.isPlaying)
        {
            source.Stop();
        }
    }

    public void StopAll()
    {
        foreach (var source in _effectsDict.Keys)
        {
            Stop(source);
        }

        foreach (var source in _audioDict.Keys)
        {
            Stop(source);
        }
    }

    public void UnPause(Audio.AudioType type)
    {
        if (TryGetSource(type, out var source) && !source.isPlaying)
        {
            source.UnPause();
        }
    }
}
