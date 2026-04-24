using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using UniRx;
using UnityEngine;

public class TerminalInterraction : MonoBehaviour
{
    [SerializeField] private HackingTerminal _hackingTerminal;
    [SerializeField] private GameObject _objectToToggle;
    [SerializeField] private GameObject _terminalOnObject;
    [SerializeField] private GameObject _terminalOffObject;
    void Start()
    {
        _hackingTerminal.CanHack
            .Subscribe(isActive => 
            {
                if (_objectToToggle != null)
                    _objectToToggle.SetActive(isActive);
                
                if (_terminalOnObject != null)
                    _terminalOnObject.SetActive(isActive);
                
                if (_terminalOffObject != null)
                    _terminalOffObject.SetActive(!isActive);
                

                Debug.Log($"Объект изменен, статус терминала: {isActive}");
            })
            .AddTo(this);
    }
    
    
}
