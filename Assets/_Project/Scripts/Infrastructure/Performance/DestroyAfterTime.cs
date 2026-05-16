using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float _lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, _lifetime);
    }
}
