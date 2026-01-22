using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Hacking.Terminal
{
    public class HackingTerminal : MonoBehaviour
    {
        private List<GameUnit> _hackableUnits;
        private HackingService _hackingService;
        private bool _isPlayerInside;

        [Inject]
        public void Construct(HackingService hackingService)
        {
            _hackingService = hackingService;
        }

        public void Initialize(List<GameUnit> units)
        {
            _hackableUnits = units;
            Debug.Log($"Терминал инициализирован. Доступно целей: {_hackableUnits.Count}");
        }

        private void OnTriggerEnter(Collider other)
        {
        
            if (other.TryGetComponent(out GameUnit unit) && unit.CompareTag("Player")) 
            {
                _isPlayerInside = true;
                ShowInteractionUI();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInside = false;
                HideInteractionUI();
            }
        }

        private void Update()
        {
            if (_isPlayerInside && Input.GetKeyDown(KeyCode.E))
            {
                OpenTargetSelectionMenu();
            }
        }

        private void OpenTargetSelectionMenu()
        {
        
            Debug.Log("Открыто меню выбора цели...");
        
            if (_hackableUnits.Count > 0 && _hackableUnits[0] != null)
            {
                OnTargetSelected(_hackableUnits[0]);
            }
        }

        public void OnTargetSelected(GameUnit target)
        {
            Debug.Log($"Начат взлом {target.name}");
        }
    
        private void ShowInteractionUI() => Debug.Log("UI: [E] Взломать");
        private void HideInteractionUI() => Debug.Log("UI: Скрыто");
    }
}
