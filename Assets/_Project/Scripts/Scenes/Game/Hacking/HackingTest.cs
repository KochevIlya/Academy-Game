using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class HackingTest : MonoBehaviour 
{
    public HackingService _hackingService;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            var dummy = new GameObject("TestTarget").AddComponent<HackableComponent>();
            _hackingService.StartHacking(dummy);
        }
    }
}
