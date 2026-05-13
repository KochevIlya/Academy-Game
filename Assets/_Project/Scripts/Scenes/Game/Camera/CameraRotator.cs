using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [Header("Настройки вращения")]
    [SerializeField] private float _angleRange = 45f;
    [SerializeField] private float _timeToRotate = 1f;
    [SerializeField] private float _pauseTime = 0.5f;

    private Quaternion _startRotation;

    private void Start()
    {
        _startRotation = transform.localRotation;
        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(MoveToAngle(_angleRange));
            yield return new WaitForSeconds(_pauseTime);

            yield return StartCoroutine(MoveToAngle(-_angleRange));
            yield return new WaitForSeconds(_pauseTime);
        }
    }

    private IEnumerator MoveToAngle(float targetAngle)
    {
        float elapsed = 0f;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation = _startRotation * Quaternion.Euler(0, targetAngle, 0);

        while (elapsed < _timeToRotate)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _timeToRotate;
            float smoothedT = Mathf.SmoothStep(0, 1, t);
            
            transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, smoothedT);
            yield return null;
        }
        
        transform.localRotation = toRotation;
    }
}
