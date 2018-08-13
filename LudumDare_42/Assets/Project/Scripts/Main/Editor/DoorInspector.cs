using UnityEngine;
using System.Collections;
using UnityEditor;
using Main.Game;

[CustomEditor(typeof(WallDoor))]
public class DoorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WallDoor myScript = (WallDoor)target;
        if (GUILayout.Button("Interact"))
        {
            myScript.ForceInteract();
        }
    }
}