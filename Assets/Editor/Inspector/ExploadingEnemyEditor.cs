using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExplosiveEnemy))]
public class ExploadingEnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Expload"))
        {
            ExplosiveEnemy enemy = target as ExplosiveEnemy;
            enemy.StartTicking();
        }
    }
}
