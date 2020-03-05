using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(ShooterEnemy))]
public class ShootingEnemyEditor : Editor
{
    ShooterEnemy script;

    private void Awake()
    {
        script = target as ShooterEnemy;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Shooting is active: " + script.GetActive());

        if(GUILayout.Button("Activate"))
        {
            script.TurnActive();
        }

        if (GUILayout.Button("Fire Up Projectile"))
        {
            script.FireUpProjectile();
        }

    }
}
