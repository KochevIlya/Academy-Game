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
        
        private CombatZone _combatZone;
        
        [Inject] private ITacticalHacking _tacticalHacking;
        
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

        public CombatZone GetCombatZone => _combatZone;
        public void SetCombatZone(CombatZone combatZone) => _combatZone = combatZone; 
        
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
                _tacticalHacking.SetHackingZoneStatus(true);
                _tacticalHacking.SetContext(WarZoneTransform, _combatZone.GetHackableObjects(), _combatZone);
                ShowInteractionUI();
            }
            else
            {
                _isActive = false;
                _tacticalHacking.SetHackingZoneStatus(false);
                _tacticalHacking.ClearContext();
                HideInteractionUI();
            }
        }

        public void SetHackingStatus(bool canHack)
        {
            _canHack.Value = canHack;   
        }
    }
}
