using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AmmoPickup))]
public class AmmoPickupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        AmmoPickup myScript = (AmmoPickup)target;
        if (GUILayout.Button("Update"))
        {
            myScript.UpdateScript();
        }
    }
}
