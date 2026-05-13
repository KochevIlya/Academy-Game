using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SoundService : ISoundService
{
    private readonly AudioSource _globalAudioSource;
    private readonly AudioSource _warAudioSource;
    private float _volume;
    public SoundService([Inject(Id = "Global")]AudioSource globalAudioSource, [Inject(Id = "War")]AudioSource warAudioSource)
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
            
            if (_globalAudioSource != null)
            {
                _globalAudioSource.volume = _volume;
            }
        }
    }

    public void PlayGlobal()
    {
        if (_globalAudioSource != null && !_globalAudioSource.isPlaying)
        {
            _globalAudioSource.Play();
        }
    }

    public void StopGlobal()
    {
        if (_globalAudioSource != null && _globalAudioSource.isPlaying)
        {
            _globalAudioSource.Stop();
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

    public void Play()
    {
        if (_globalAudioSource != null && !_globalAudioSource.isPlaying)
        {
            _globalAudioSource.Play();
        }
    }

    public void Stop()
    {
        if (_globalAudioSource != null && _globalAudioSource.isPlaying)
        {
            _globalAudioSource.Stop();
        }
    }
}
