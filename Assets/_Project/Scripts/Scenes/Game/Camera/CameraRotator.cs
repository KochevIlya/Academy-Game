using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [Header("Настройки вращения")]
    [SerializeField] private float _angleRange = 45f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _pauseTime = 0.5f;

    private Quaternion _startRotation;
    private float _timer;

    private void Start()
    {
        _startRotation = transform.localRotation;
    }

    private void Update()
    {
        float pingPong = Mathf.PingPong(Time.time * _speed, 1f);
        
        float smoothedStep = Mathf.SmoothStep(0f, 1f, pingPong);

        float currentAngle = Mathf.Lerp(-_angleRange, _angleRange, smoothedStep);

        transform.localRotation = _startRotation * Quaternion.Euler(0, currentAngle, 0);
    }
}
