using UnityEngine;
using System;

public class EntityIdentifier : MonoBehaviour
{
    [SerializeField] private string _id;
    public string ID => _id;

    [ContextMenu("Generate Unique ID")]
    public void GenerateID()
    {
        _id = Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_id))
        {
            GenerateID();
        }
    }
}