using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class PatrolPathSaveHelper
{
    public static List<WaypointData> GetSaveData(PatrolPath path)
    {
        var data = new List<WaypointData>();
        if (path == null) return data;

        foreach (var point in path.GetPatrolPoints()) 
        {
            if (point.PointTransform != null)
            {
                data.Add(new WaypointData
                {
                    pos = point.PointTransform.position,
                    waitTime = point.WaitTime
                });
            }
        }
        return data;
    }

    public static PatrolPath RestorePath(List<WaypointData> savedData, string unitId)
    {
        if (savedData == null || savedData.Count == 0) return null;

        GameObject pathRoot = new GameObject($"RestoredPath_{unitId}");
        PatrolPath newPath = pathRoot.AddComponent<PatrolPath>();

        var pointsList = newPath.GetPatrolPoints();

        for (int i = 0; i < savedData.Count; i++)
        {
            GameObject pointObj = new GameObject($"Point_{i}");
            pointObj.transform.SetParent(pathRoot.transform);
            pointObj.transform.position = savedData[i].pos;

            pointsList.Add(new PatrolPath.PatrolPoint
            {
                PointTransform = pointObj.transform,
                WaitTime = savedData[i].waitTime
            });
        }

        newPath.InitializeCache();

        return newPath;
    }
}
