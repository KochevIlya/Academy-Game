using System.Collections.Generic;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Hacking.Terminal
{
    public class HackingTerminal : MonoBehaviour
    {
        [Inject] private HackingService _hackingService;
        [Inject] private HackableSelector _hackableSelector;
        public Transform WarZoneTransform;
        
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
        // }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                _isActive = true;
                Debug.Log($"Внутри зоны ");

                _hackingService.SetHackingZoneStatus(true);
                _hackableSelector.SetContext(WarZoneTransform);
                ShowInteractionUI();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                _isActive = false;
                Debug.Log($"Ушли из зоны");
                _hackingService.SetHackingZoneStatus(false);
                _hackableSelector.ClearContext();
                HideInteractionUI();
            }
        }
    
        private void ShowInteractionUI() => Debug.Log("UI: [E] Взломать");
        private void HideInteractionUI() => Debug.Log("UI: Скрыто");
        
    }
}
