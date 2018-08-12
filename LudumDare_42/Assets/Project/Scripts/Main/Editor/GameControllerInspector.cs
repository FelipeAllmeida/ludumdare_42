using UnityEngine;
using System.Collections;
using UnityEditor;
using Main.Game;

namespace Main
{
    [CustomEditor(typeof(GameController))]
    public class GameControllerInspector : Editor
    {
        private int _targetX;
        private int _targetY;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GameController myScript = (GameController)target;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Debug Options", EditorStyles.boldLabel);

            _targetX = EditorGUILayout.IntField(new GUIContent("X"), _targetX);
            _targetY = EditorGUILayout.IntField(new GUIContent("Y"), _targetY);

            if (GUILayout.Button("Check if have water on adjacent rooms"))
            {
                Debug.Log("Have Water: " + myScript.HaveWaterComingFromAdjacentRooms(_targetX, _targetY));
            }
        }
    }
}