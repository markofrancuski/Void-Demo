using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Boss))]
public class BossEditor : Editor
{
    Boss script;

    private void Awake()
    {
        script = target as Boss;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Shooting is active: " + script.GetActive());

        if (GUILayout.Button("Activate"))
        {
            script.TurnActive();
        }

        if (GUILayout.Button("Fire Up Projectile"))
        {
            script.Fire();
        }

    }
}
