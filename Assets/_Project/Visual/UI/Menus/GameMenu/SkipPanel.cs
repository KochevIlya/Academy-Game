using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UniRx;

public class SkipPanel : MonoBehaviour
{
    [SerializeField] private KeyCode _skipKey = KeyCode.Space;
    [SerializeField] private TextMeshProUGUI _skipText;
    
    public readonly Subject<Unit> OnSkipPressed = new Subject<Unit>();

    private void Start()
    {
        _skipText.text = _skipKey.ToString();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(_skipKey))
        {
            OnSkipPressed.OnNext(Unit.Default);
        }
    }
}
