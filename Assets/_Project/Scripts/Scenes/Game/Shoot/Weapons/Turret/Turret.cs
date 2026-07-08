using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

public class Turret : TurretBase
{
    private LaserTrigger _laserTrigger;
    [SerializeField] private Transform _towerRotatingPart;
    [SerializeField] private int _damagePerTick = 5;
    [SerializeField] private float _damageInterval = 0.1f;
    [SerializeField] private float _rotationSpeed = 25f;
    private float _damageTimer;
    
    private void Awake()
    {
        _laserTrigger = GetComponentInChildren<LaserTrigger>();
        _laserTrigger.Initialize(this);
        
        Activate();
    }
    private void Update()
    {
        if (!IsActive()) return;
        
        if (_towerRotatingPart != null)
        {
            _towerRotatingPart.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);
        }

        _damageTimer += Time.deltaTime;
    }
    
    public override void Activate()
    {
        base.Activate();
    }
    
    public void TryDamageUnit(GameUnit unit)
    {
        Debug.Log("[Turret] Trying to damage unit");
        if (!IsActive() || _damageTimer < _damageInterval) return;

        if (unit.Health != null)
        {
            unit.Health.TakeDamage(_damagePerTick);
        }

        if (_damageTimer >= _damageInterval)
        {
            _damageTimer = 0f;
        }
    }
    
    
    
}
