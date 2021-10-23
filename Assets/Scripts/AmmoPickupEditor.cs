using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;

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
#endif