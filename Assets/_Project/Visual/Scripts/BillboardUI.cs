using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform _mainCameraTransform;

    void Start() => _mainCameraTransform = Camera.main.transform;

    void LateUpdate()
    {
        // Поворачиваем UI так же, как повернута камера
        transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward,
            _mainCameraTransform.rotation * Vector3.up);
    }
}
