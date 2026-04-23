using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using UniRx;
using UnityEngine;

public class InterractionHide : MonoBehaviour
{
    [SerializeField] private HackingTerminal _hackingTerminal;
    [SerializeField] private GameObject _objectToToggle;
    void Start()
    {
        _hackingTerminal.CanHack
            .Subscribe(isActive => 
            {
                if (_objectToToggle != null)
                {
                    _objectToToggle.SetActive(isActive); 
                }
                
                Debug.Log($"Объект изменен, статус терминала: {isActive}");
            })
            .AddTo(this);
    }
    
    
}
