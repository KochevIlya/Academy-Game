using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundService

{
    float Volume { get; set; }
    void PlayGlobal();
    void StopGlobal();
    void PlayGlobalFromBeginning();
    void PlayWar();
    void StopWar();
    void PlayWarFromBeginning();

}
