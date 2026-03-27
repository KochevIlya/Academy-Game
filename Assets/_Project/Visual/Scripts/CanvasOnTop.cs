using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOnTop : MonoBehaviour
{
    
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);
    [SerializeField] private CanvasGroup _canvasGroup;
    
    private Camera _mainCamera;
    private RectTransform _rectTransform;
    
    public void Start()
    {
        if (this == null) return;
        
        _mainCamera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
        
    }
    private void LateUpdate()
    {
        
        if (gameObject == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = _targetObject.transform.position + _offset;
        Vector3 screenPoint = _mainCamera.WorldToScreenPoint(targetPosition);
            
        bool isBehind = screenPoint.z < 0; 
        _canvasGroup.alpha = isBehind ? 0 : 1;

        if (!isBehind)
            _rectTransform.position = screenPoint;
            
    }
}
