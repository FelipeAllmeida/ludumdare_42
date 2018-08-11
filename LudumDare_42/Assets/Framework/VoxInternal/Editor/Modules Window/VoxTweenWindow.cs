using UnityEngine;
using UnityEditor;
using System.Collections;
using Vox;
using VoxInternal;
using System.Collections.Generic;

namespace VoxInternal.Editor
{
	/// <summary>
	/// Window used to debug playing tweens nodes informations.
	/// </summary>
	public class VoxTweenWindow : EditorWindow 
	{
		private static VoxTweenWindow _window;
		private static Vector2 _scrollPosition = Vector2.zero;
		private static Dictionary<int,bool> _dictNodesList = new Dictionary<int, bool>();

		[MenuItem("Vox/Modules/Tween")]
		/// <summary>
		/// Initialize the unity editor window.
		/// </summary>
		static private void Initialize () 
		{
			if(_window == null)
				GetWindow ();
		}
		/// <summary>
		/// Unity function to get editor window reference.
		/// </summary>
		static private void GetWindow()
		{
			_window = (VoxTweenWindow)EditorWindow.GetWindow<VoxTweenWindow> ();

			if (_window != null)
			{
				_window.titleContent.text = "Tween";
				_window.titleContent.image = (Texture)AssetDatabase.LoadAssetAtPath("Assets/VoxFramework/Resources/VoxEditorData/VoxLogo32.png", typeof(Texture));
				_window.autoRepaintOnSceneChange = true;

			}
		}
		/// <summary>
		/// Called when windows is focused/selected.
		/// </summary>
		private void OnFocus() 
		{
			if(_window == null)
				GetWindow ();
		}
		/// <summary>
		/// Function that draws information on this window.
		/// </summary>
		private void OnGUI()
		{
			if(_window == null)
				GetWindow ();
			
			DrawHeaderBasicInfo ();

			if (EditorApplication.isPlaying) 
			{
				DebugTween ();			
			}
		}
		/// <summary>
		/// Function that draws the tiitle information of the editor.
		/// </summary>
		private void DrawHeaderBasicInfo()
		{
			GUI.Box (new Rect(0, 0, _window.position.width, 24), "");

			GUIStyle _labelStyle = new GUIStyle(GUI.skin.label);
			_labelStyle.fontSize = 12;
			_labelStyle.normal.textColor = Color.black;

			EditorGUILayout.LabelField ("Tween Debug Window", _labelStyle);

			_labelStyle.fontSize = 11;

			EditorGUILayout.Space ();

			if (EditorApplication.isPlaying == false) 
			{
				GUILayout.Label ("Debug running tween nodes in play mode with this window.");

				EditorGUILayout.Space ();
			}
		}
		/// <summary>
		/// Function that updates de debug of the tween nodes.
		/// </summary>	
		private void DebugTween()
		{
			GUI.Box (new Rect(0, GUILayoutUtility.GetLastRect().yMax, _window.position.width, _window.position.height), "");
			EditorGUILayout.LabelField ("Nodes Count: " + TweenModule.instance.listOfNodes.Count);

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Hide All"))
			{
				_dictNodesList.Clear ();
			}
			else if (GUILayout.Button ("Show All")) 
			{
				_dictNodesList.Clear ();

				for (int i = TweenModule.instance.listOfNodes.Count - 1; i >= 0; i--) 
				{
					int __id = TweenModule.instance.listOfNodes[i].id;

					_dictNodesList.Add (__id, true);
				}
			}

			GUILayout.EndHorizontal ();

			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, true);
			
			for (int i = TweenModule.instance.listOfNodes.Count - 1; i >= 0; i--)
			{
				int __id = TweenModule.instance.listOfNodes[i].id;

				if (_dictNodesList.ContainsKey (__id) == false)
					_dictNodesList.Add (__id, false);
					
				GUILayout.BeginHorizontal ();

				if (_dictNodesList [__id] == false) {
					GUI.color = Color.grey;
				} else {
					GUI.color = Color.white;
				}

				if (GUILayout.Button ("Tween Node " + __id + ": " + TweenModule.instance.listOfNodes[i].delegatedCallback.Method.ReflectedType.FullName)) {
					_dictNodesList [__id] = !_dictNodesList [__id];
				}

				GUILayout.FlexibleSpace ();

				GUILayout.EndHorizontal ();

				if (_dictNodesList [__id]) {
					GUI.color = Color.white;

					DisplayTweenNodeData ( TweenModule.instance.listOfNodes[i] );
				}
			}

			EditorGUILayout.EndScrollView();

		}
		/// <summary>
		/// Function that display the information of passed node.
		/// </summary>
		public void DisplayTweenNodeData(TweenNode __node)
		{
			GUILayout.Label ("\tUsedByClass: " + __node.delegatedCallback.Method.ReflectedType.FullName);
			GUILayout.Label ("\tMethod: " + __node.delegatedCallback.Method.Name);
			GUILayout.Label ("\tStartValue:" + __node.startValue);
			GUILayout.Label ("\tFinalValue:" + __node.endValue);
			GUILayout.Label ("\tDuration:" + __node.duration);
			GUILayout.Label ("\tCurrent Time Step:" + __node.currentTimeStep);
			GUILayout.Label ("\tCurrent State:" + __node.currentState);
			GUILayout.Label ("\tUpdate Mode:" + __node.updateMode);
			GUILayout.Label ("\tLoop Mode:" + __node.loopMode);
			if (__node.loopMode != TweenLoopType.NONE) 
			{
				if(__node.loopMode == TweenLoopType.PING_PONG)
				{
					string __text = "\tPingPongState: NORMAL";

					if (__node.isPingPongGoingBack) 
					{
						__text = "\tPingPongState: Reverse";
					}
					GUILayout.Label (__text);
				}
			}
			GUILayout.Label ("\tTimeScale Mode:" + __node.useTimeScale);

			if (__node.associatedObject) 
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("\tAssociated GameObject:" + __node.associatedObject.name);

				if (GUILayout.Button ("Select")) 
				{
					Selection.activeGameObject = __node.associatedObject;
				}
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
			}
		}
	}
}
