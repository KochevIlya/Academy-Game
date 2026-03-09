using System.Collections.Generic;
using UnityEngine;

public static class PathRegistry
{
    private static readonly Dictionary<string, PatrolPath> _paths = new Dictionary<string, PatrolPath>();

    public static void Register(string id, PatrolPath path) => _paths[id] = path;
    public static void Unregister(string id) => _paths.Remove(id);
    
    public static PatrolPath Get(string id)
    {
        return _paths.TryGetValue(id, out var path) ? path : null;
    }
}

[RequireComponent(typeof(PatrolPath))]
public class EntityIdentifier : MonoBehaviour
{
    public string ID;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(ID))
        {
            PathRegistry.Register(ID, GetComponent<PatrolPath>());
        }
        else
        {
            Debug.LogWarning($"[EntityIdentifier] ID is empty on {gameObject.name}!", this);
        }
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(ID))
        {
            PathRegistry.Unregister(ID);
        }
    }

    [ContextMenu("Generate Unique ID")]
    private void GenerateId()
    {
        ID = System.Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
    }
}