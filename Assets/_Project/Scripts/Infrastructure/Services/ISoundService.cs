using System.Collections;
using System.Collections.Generic;
using _Project.Sounds;
using UnityEngine;

public interface ISoundService

{
    float Volume { get; set; }
    float EffectVolume { get; set; }
    void PlayGlobal();
    void StopGlobal();
    void PlayGlobalFromBeginning();
    void PlayWar();
    void StopWar();
    void PlayWarFromBeginning();
    void Play(Audio.AudioType type, bool fromBeginning = false);
    void Stop(Audio.AudioType type);

}
