using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Scenes.Game.Unit;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Hacking.Terminal
{
    public class HackingTerminal : MonoBehaviour
    {
        [Inject] private HackingService _hackingService;
        [Inject] private HackableSelector _hackableSelector;
        public Transform WarZoneTransform;
        
        private readonly BoolReactiveProperty _canHack = new BoolReactiveProperty(false);
        public IReadOnlyReactiveProperty<bool> CanHack => _canHack;
        
        public bool _isActive = false;
        public string _id;
        
        
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.tag == "Player")
        //     {
        //         _isActive = true;
        //         Debug.Log($"Внутри зоны ");
        //
        //     _hackingService.SetHackingZoneStatus(true);
        //     _hackableSelector.SetContext(WarZoneTransform);
        //     ShowInteractionUI();
        //     }
        // }a
        private void Start()
        {
            _canHack.Value = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetActiveStatus(true);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetActiveStatus(false);
            }
        }

        private void ShowInteractionUI()
        {
            //Debug.Log("UI: [E] Взломать");
        } 
        private void HideInteractionUI() {
            //Debug.Log("UI: Скрыто");
        }
        
        private void SetActiveStatus(bool active)
        {
            _isActive = active;
            if (active)
            {
                _isActive = true;
                _hackingService.SetHackingZoneStatus(true);
                _hackableSelector.SetContext(WarZoneTransform);
                ShowInteractionUI();
            }
            else
            {
                _isActive = false;
                _hackingService.SetHackingZoneStatus(false);
                _hackableSelector.ClearContext();
                HideInteractionUI();
            }
        }

        public void SetHackingStatus(bool canHack)
        {
            _canHack.Value = canHack;   
        }
    }
}
