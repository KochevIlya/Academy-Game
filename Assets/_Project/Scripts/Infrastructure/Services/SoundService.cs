using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SoundService : ISoundService
{
    private readonly AudioSource _globalAudioSource;
    private readonly AudioSource _warAudioSource;
    
    private readonly IReadOnlyDictionary<AudioType, AudioSource> _audioDict;
    
    private float _volume;
    private float _effectsVolume;
    public SoundService(
        [Inject(Id = "Global")]AudioSource globalAudioSource
        ,[Inject(Id = "War")]AudioSource warAudioSource
        ,IReadOnlyDictionary<AudioType, AudioSource> audioDict
        )
        
    {
        _globalAudioSource = globalAudioSource;
        _warAudioSource = warAudioSource;
        _volume = globalAudioSource.volume;
        StopWar();
        if (_globalAudioSource != null)
        {
            _globalAudioSource.volume = _volume;
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

    public void Play(AudioType type, bool fromBeginning = false)
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
    private bool TryGetSource(AudioType type, out AudioSource source)
    {
        if (_audioDict.TryGetValue(type, out source) && source != null)
        {
            return true;
        }
        
        Debug.LogWarning($"[Sound Service] AudioSource for type {type} not found or null!");
        return false;
    }
    public void Stop(AudioType type)
    {
        if (TryGetSource(type, out var source) && source.isPlaying)
        {
            source.Stop();
        }
    }
    public void UnPause(AudioType type)
    {
        if (TryGetSource(type, out var source) && !source.isPlaying)
        {
            source.UnPause();
        }
    }
}
