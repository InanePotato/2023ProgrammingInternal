using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CustomEditor(typeof(SpawnerScript))]
public class SpawnerRangesEditor : Editor
{
    private void OnSceneGUI()
    {
        SpawnerScript spawnerScript = (SpawnerScript)target;

        if (spawnerScript == null)
        {
            return;
        }

        Vector2 spawnRange = spawnerScript.spawnRange;

        // draw spawn range wireframe
        var t = target as SpawnerScript;
        var tr = t.transform;
        var pos = tr.position;
        pos = new Vector3(pos.x, pos.y, pos.z);

        Vector3[] points = new Vector3[5];
        points[0] = new Vector3(pos.x + spawnRange.x, pos.y + spawnRange.y, pos.z);
        points[1] = new Vector3(pos.x - spawnRange.x, pos.y + spawnRange.y, pos.z);
        points[2] = new Vector3(pos.x - spawnRange.x, pos.y - spawnRange.y, pos.z);
        points[3] = new Vector3(pos.x + spawnRange.x, pos.y - spawnRange.y, pos.z);
        points[4] = new Vector3(pos.x + spawnRange.x, pos.y + spawnRange.y, pos.z);

        Handles.color = Color.green;
        Handles.DrawPolyLine(points);
    }
}
