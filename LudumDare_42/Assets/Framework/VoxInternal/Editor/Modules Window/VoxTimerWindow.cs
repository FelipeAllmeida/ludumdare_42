using UnityEngine;
using UnityEditor;
using System.Collections;
using Vox;
using VoxInternal;
using System.Collections.Generic;

namespace VoxInternal.Editor
{
	/// <summary>
	/// Window used to debug playing timer nodes informations.
	/// </summary>
	public class VoxTimerWindow : EditorWindow 
	{
		private static VoxTimerWindow _window;
		//private static Vector2 _scrollPosition = Vector2.zero;
		//private static Dictionary<int,bool> _dictNodesList = new Dictionary<int, bool>();

		[MenuItem("Vox/Modules/Timer")]
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
			_window = (VoxTimerWindow)EditorWindow.GetWindow<VoxTimerWindow> ();

			if (_window != null)
			{
				_window.titleContent.text = "Timer";
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
				DebugTimerNodes ();			
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

			EditorGUILayout.LabelField ("Timer Debug Window", _labelStyle);

			_labelStyle.fontSize = 11;

			EditorGUILayout.Space ();

			if (EditorApplication.isPlaying == false) 
			{
				GUILayout.Label ("Debug running timer nodes in play mode with this window.");

				EditorGUILayout.Space ();
			}
		}
		/// <summary>
		/// Function that updates de debug of the timer nodes.
		/// </summary>	
		private void DebugTimerNodes()
		{
//			GUI.Box (new Rect(0, GUILayoutUtility.GetLastRect().yMax, _window.position.width, _window.position.height), "");
//			EditorGUILayout.LabelField ("Update Nodes Count: " + TimerModule.instance.dictTimerNodes[UpdateTimeType.UPDATE].Count);
//			EditorGUILayout.LabelField ("Fixed Update Nodes Count: " + TimerModule.instance.dictTimerNodes[UpdateTimeType.FIXED_UPDATE].Count);
//			EditorGUILayout.LabelField ("Late Update Nodes Count: " + TimerModule.instance.dictTimerNodes[UpdateTimeType.LATE_UPDATE].Count);
//
//			GUILayout.BeginHorizontal ();
//
//			if (GUILayout.Button ("Hide All"))
//			{
//				_dictNodesList.Clear ();
//			}
//			else if (GUILayout.Button ("Show All")) 
//			{
//				_dictNodesList.Clear ();
//
//				foreach(KeyValuePair<UpdateTimeType, List<TimerNode>> __dict in TimerModule.instance.dictTimerNodes)
//				{
//					for (int i = __dict.Value.Count - 1; i >= 0; i--) 
//					{
//						int __id = __dict.Value[i].id;
//
//						_dictNodesList.Add (__id, true);
//					}
//				}
//			}
//
//			GUILayout.EndHorizontal ();
//
//			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, true);
//
//			foreach(KeyValuePair<UpdateTimeType, List<TimerNode>> __dict in TimerModule.instance.dictTimerNodes)
//			{
//				for (int i = __dict.Value.Count - 1; i >= 0; i--) 
//				{
//					int __id = __dict.Value[i].id;
//
//					if (_dictNodesList.ContainsKey (__id) == false)
//						_dictNodesList.Add (__id, false);
//					
//					GUILayout.BeginHorizontal ();
//
//					if (_dictNodesList [__id] == false) 
//					{
//						GUI.color = Color.grey;
//					} 
//					else 
//					{
//						GUI.color = Color.white;
//					}
//
//					if (GUILayout.Button ("Timer Node " + __id + ": " + __dict.Value[i].delegatedCallback.Method.ReflectedType.FullName)) 
//					{
//						_dictNodesList [__id] = !_dictNodesList [__id];
//					}
//
//					GUILayout.FlexibleSpace ();
//
//					GUILayout.EndHorizontal ();
//
//					if (_dictNodesList [__id]) 
//					{
//						GUI.color = Color.white;
//
//						DisplayNodeData (__dict.Value[i]);
//					}
//				}
//			}
//
//			EditorGUILayout.EndScrollView();
		}
		/// <summary>
		/// Function that display the information of passed node.
		/// </summary>
		public void DisplayNodeData(TimerNode __node)
		{
			GUILayout.Label ("\tUsedByClass: " + __node.delegatedCallback.Method.ReflectedType.FullName);
			GUILayout.Label ("\tMethod: " + __node.delegatedCallback.Method.Name);
			GUILayout.Label ("\tFrames to wait:" + __node.framesToWait);
			GUILayout.Label ("\tFrame Counter:" + __node.frameCounter);
			GUILayout.Label ("\tTime to wait:" + __node.timeToWait);
			GUILayout.Label ("\tCurrent Time Step:" + __node.currentTimeStep);
			GUILayout.Label ("\tCurrent State:" + __node.currentState);
			GUILayout.Label ("\tUpdate Mode:" + __node.updateMode);
			GUILayout.Label ("\tIs Loop:" + __node.usingLoop);
			GUILayout.Label ("\tLoop Iteration:" + __node.loopIteration);

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
