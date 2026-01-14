using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class HackingTest : MonoBehaviour 
{
    [Inject]
    public HackingService _hackingService;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var dummy = new GameObject("TestTarget").AddComponent<HackableComponent>();
            _hackingService.StartHacking(dummy);
        }
    }
}
