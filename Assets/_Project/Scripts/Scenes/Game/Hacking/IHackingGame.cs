using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IHackingGame
{
    public IObservable<Unit> StartHackingGame();
    public void StopHackingGame();
}
