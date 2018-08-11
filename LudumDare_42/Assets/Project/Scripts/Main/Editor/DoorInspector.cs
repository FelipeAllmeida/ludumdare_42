﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Door))]
public class DoorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Door myScript = (Door)target;
        if (GUILayout.Button("Interact"))
        {
            myScript.Interact();
        }
    }
}