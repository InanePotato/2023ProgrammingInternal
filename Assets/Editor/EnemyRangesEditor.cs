using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CustomEditor(typeof(EnemyScript))]
public class EnemyRangesEditor : Editor
{
    public void OnSceneGUI()
    {
        EnemyScript enemyScript = (EnemyScript)target;

        if (enemyScript == null)
        {
            return;
        }

        //float wanderRange = enemyScript.wanderRange;
        Vector2 wanderOffset = enemyScript.movementOffset;
        float targetRange = enemyScript.targetingRange;
        float attackRange = enemyScript.attackRange;

        // draw wander range wireframe
        var t = target as EnemyScript;
        var tr = t.transform;
        var pos = tr.position;
        pos = new Vector3(pos.x, pos.y - 0.06f, pos.z);

        if (enemyScript.movmentType == EnemyScript.MovmentType.Raom)
        {
            Vector3[] points = new Vector3[5];
            points[0] = new Vector3(pos.x + wanderOffset.x, pos.y + wanderOffset.y, pos.z);
            points[1] = new Vector3(pos.x - wanderOffset.x, pos.y + wanderOffset.y, pos.z);
            points[2] = new Vector3(pos.x - wanderOffset.x, pos.y - wanderOffset.y, pos.z);
            points[3] = new Vector3(pos.x + wanderOffset.x, pos.y - wanderOffset.y, pos.z);
            points[4] = new Vector3(pos.x + wanderOffset.x, pos.y + wanderOffset.y, pos.z);

            Handles.color = Color.green;
            Handles.DrawPolyLine(points);
            Handles.Label(new Vector3(pos.x - 0.15f, pos.y + wanderOffset.y + 0.05f, pos.z), "Wander Range");
        }
        else if (enemyScript.movmentType == EnemyScript.MovmentType.Linear)
        {
            Handles.color = Color.green;
            Vector3 p1 = new Vector3(pos.x + wanderOffset.x, pos.y + wanderOffset.y, pos.z);
            Vector3 p2 = new Vector3(pos.x - wanderOffset.x, pos.y - wanderOffset.y, pos.z);
            Handles.DrawLine(p1, p2);
            Handles.Label(new Vector3(p1.x - 0.15f, p1.y + 0.05f, pos.z), "Wander Range");
        }

        // draw target range wireframe
        var t1 = target as EnemyScript;
        var tr1 = t1.transform.GetChild(0).gameObject.transform;
        var pos1 = tr1.position;
        pos1 = new Vector3(pos1.x, pos1.y - 0.06f, pos1.z);

        Handles.color = Color.blue;
        Handles.DrawWireDisc(pos1, tr1.forward, targetRange);
        Handles.Label(new Vector3(pos.x - 0.15f, pos.y + targetRange + 0.03f, pos.z), "Target Range");

        // draw attack range wireframe
        var t2 = target as EnemyScript;
        var tr2 = t2.transform.GetChild(0).gameObject.transform;
        var pos2 = tr2.position;
        pos2 = new Vector3(pos2.x, pos2.y - 0.06f, pos2.z);

        Handles.color = Color.red;
        Handles.DrawWireDisc(pos2, tr2.forward, attackRange);
        Handles.Label(new Vector3(pos.x - 0.15f, pos.y + attackRange + 0.03f, pos.z), "Attack Range");
    }
}
