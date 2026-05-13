using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundService : ISoundService
{
    private readonly AudioSource _audioSource;
    private float _volume;
    public SoundService(AudioSource audioSource)
    {
        _audioSource = audioSource;
        _volume = audioSource.volume;
        
        if (_audioSource != null)
        {
            _audioSource.volume = _volume;
        }
    }
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Mathf.Clamp01(value); 
            
            if (_audioSource != null)
            {
                _audioSource.volume = _volume;
            }
        }
    }
    public void Play()
    {
        if (_audioSource != null && !_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }

    public void Stop()
    {
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}
