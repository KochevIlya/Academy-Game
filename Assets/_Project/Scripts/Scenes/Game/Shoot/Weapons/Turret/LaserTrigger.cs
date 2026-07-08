using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;


public class LaserTrigger : MonoBehaviour
{
    private Turret _parentTurret;
    private bool _isActive;

    public void Initialize(Turret parent)
    {
        _parentTurret = parent;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isActive) return;
        if (!other.CompareTag("HitBox")) return;

        if (_parentTurret == null) 
        {
            Debug.LogError("[LaserTrigger] Сбой! _parentTurret равен NULL. Ссылка потерялась или не передалась.");
            return;
        }

        if (other.transform.IsChildOf(_parentTurret.transform)) 
        {
            Debug.LogWarning($"[LaserTrigger] Игнор: {other.name} почему-то считается дочерним объектом турели.");
            return;
        }

        GameUnit unit = other.GetComponent<GameUnit>() ?? other.transform.root.GetComponent<GameUnit>();

        if (unit == null)
        {
            Debug.LogWarning($"[LaserTrigger] Ошибка: На {other.name} в этот раз не нашли GameUnit.");
            return;
        }
        
        
        Debug.Log($"[LaserTrigger] Отлично! Передаем {unit.name} в метод TryDamageUnit.");
        
        _parentTurret.TryDamageUnit(unit);
    }

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }
    
}
