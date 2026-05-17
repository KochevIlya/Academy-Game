using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDelayOnAwake : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _seconds = 1f;

    private void Start()
    {
        
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if (_audioSource != null)
            StartCoroutine(PlayWithDelay());
        else
            Debug.LogError($"AudioSource не найден на объекте {gameObject.name}!", this);
    }

    private IEnumerator PlayWithDelay()
    {
        yield return new WaitForSeconds(_seconds);
        _audioSource.Play();
    }

}
