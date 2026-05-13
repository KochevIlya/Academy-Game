using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundService

{
    float Volume { get; set; }
    void Play();
    void Stop();
    
}
