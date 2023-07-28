using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CustomEditor(typeof(PlayerAttackScript))]
public class PlayerRangesEditor : Editor
{
    public void OnSceneGUI()
    {
        PlayerAttackScript playerAttackScript = (PlayerAttackScript)target;

        if (playerAttackScript == null)
        {
            return;
        }

        float attackRange = playerAttackScript.attackRange;

        var t = target as PlayerAttackScript;
        var tr = t.transform;
        var pos = tr.position;
        pos = new Vector3(pos.x, pos.y, pos.z);

        Handles.color = Color.red;
        Handles.DrawWireDisc(pos, tr.forward, attackRange);
    }
}
