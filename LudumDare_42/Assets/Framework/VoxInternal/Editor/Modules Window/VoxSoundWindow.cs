using UnityEngine;
using UnityEditor;
using System.Collections;
using Vox;
using VoxInternal;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace VoxInternal.Editor
{
	/// <summary>
	/// Window used to debug playing sound informations as well as saving mixers to be used in the Sound Module of the framework.
	/// </summary>
	public class VoxSoundWindow : EditorWindow 
	{
		private static VoxSoundWindow _window;
		private static Vector2 _scrollPosition = Vector2.zero;
		private static Dictionary<int,bool> _dictNodesList = new Dictionary<int, bool>();
		private static EditorSoundData _soundData;
		private static string _dataPath = "Assets/VoxFramework/Resources/VoxSoundData/VoxSoundSavedData.asset";

		[MenuItem("Vox/Modules/Sound")]
		/// <summary>
		/// Initialize the unity editor window.
		/// </summary>
		static private void Initialize () 
		{
			if(_window == null)
				GetWindow ();

			_window.titleContent.text = "Sound";
			_window.titleContent.image = (Texture)AssetDatabase.LoadAssetAtPath ("Assets/VoxFramework/Resources/VoxEditorData/VoxLogo32.png", typeof(Texture));
			_window.autoRepaintOnSceneChange = true;

		}

		/// <summary>
		/// Unity function to get editor window reference.
		/// </summary>
		static private void GetWindow()
		{
			_window = (VoxSoundWindow)EditorWindow.GetWindow<VoxSoundWindow> ();
		}

		/// <summary>
		/// Get or create stored sound data.
		/// </summary>
		static private void GetSoundData()
		{
			_soundData = (EditorSoundData)AssetDatabase.LoadAssetAtPath(_dataPath, typeof(EditorSoundData));

			if(_soundData == null)
			{
				_soundData = ScriptableObject.CreateInstance<EditorSoundData>();

				AssetDatabase.CreateAsset(_soundData, AssetDatabase.GenerateUniqueAssetPath(_dataPath));
				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh();
			}

			if (_soundData.mixersList == null) 
			{
				_soundData.mixersList = new List<AudioMixer>();
			}
		}

		/// <summary>
		/// Called when windows is focused/selected.
		/// </summary>
		private void OnFocus() 
		{
			if(_window == null)
				GetWindow ();

			if(_soundData == null)
				GetSoundData ();
		}

		/// <summary>
		/// Function that draws information on this window.
		/// </summary>
		private void OnGUI()
		{
			if(_window == null)
				GetWindow ();
			
			if(_soundData == null)
				GetSoundData ();
			
			DrawHeaderBasicInfo ();


			if (EditorApplication.isPlaying) 
			{
				DebugSoundNodes ();			
			} 
			else 
			{
				EditSoundData ();
			}
		}

		/// <summary>
		/// Function that allows modifications on the framework Sound Data.
		/// </summary>
		public void EditSoundData()
		{
			if (_soundData == null)
				GetSoundData ();

			GUILayout.Label ("Audio Mixers in Game: ");

			EditorGUI.BeginChangeCheck ();

			for (int i = 0; i < _soundData.mixersList.Count; i++) 
			{
				GUI.color = Color.white;

				GUILayout.BeginHorizontal ();

				GUILayout.Label ("Element" + i + ":");

				_soundData.mixersList [i] = (AudioMixer)EditorGUILayout.ObjectField (_soundData.mixersList [i], typeof(AudioMixer), false);

				GUI.color = Color.red;

				if(GUILayout.Button( "x"))
				{
					_soundData.mixersList.RemoveAt (i);
					continue;
				}

				GUILayout.FlexibleSpace ();

				GUILayout.EndHorizontal ();
			}

			GUI.color = Color.green;

			if (EditorGUI.EndChangeCheck ()) 
			{
				EditorUtility.SetDirty (_soundData);	
			}

			GUILayout.BeginHorizontal ();

			if(GUILayout.Button( "+"))
			{
				_soundData.mixersList.Add (null);
			}
			GUILayout.FlexibleSpace ();

			GUILayout.EndHorizontal ();

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

			EditorGUILayout.LabelField ("Sound Manager Window", _labelStyle);

			_labelStyle.fontSize = 11;

			EditorGUILayout.Space ();

			if (_soundData == null)
				GetSoundData ();

			if (EditorApplication.isPlaying == false) 
			{
				GUILayout.BeginHorizontal ();
				EditorGUI.BeginChangeCheck ();
					_soundData.enableSoundInEditor = GUILayout.Toggle (_soundData.enableSoundInEditor, "");
					GUILayout.Label ("Enable SOUND in EDITOR");
					GUILayout.FlexibleSpace ();
					GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();

				if (EditorGUI.EndChangeCheck ()) 
				{
					EditorUtility.SetDirty (_soundData);	
				}

				EditorGUILayout.Space ();

				GUILayout.Label ("Debug running sound nodes in play mode with this window.");

				EditorGUILayout.Space ();
			} 
			else 
			{
				if (_soundData.enableSoundInEditor == false)
				{
					GUILayout.BeginHorizontal ();
						_labelStyle.normal.textColor = Color.red;
						GUILayout.Label ("Sound is Disabled!", _labelStyle);
						GUILayout.FlexibleSpace ();
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();
				}
			}
		}

		/// <summary>
		/// Function that updates de debug of the sound nodes.
		/// </summary>	
		private void DebugSoundNodes()
		{
			GUI.Box (new Rect(0, GUILayoutUtility.GetLastRect().yMax, _window.position.width, _window.position.height), "");
			EditorGUILayout.LabelField ("Nodes Count: " + SoundModule.instance.currentNodeList.Count);

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Hide All"))
			{
				_dictNodesList.Clear ();
			}
			else if (GUILayout.Button ("Show All")) 
			{
				_dictNodesList.Clear ();

				for (int i = 0; i < SoundModule.instance.currentNodeList.Count; i++) 
				{
					int __id = SoundModule.instance.currentNodeList[i].id;

					_dictNodesList.Add (__id, true);
				}
			}

			GUILayout.EndHorizontal ();

			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, true);

			for (int i = 0; i < SoundModule.instance.currentNodeList.Count; i++) 
			{
				int __id = SoundModule.instance.currentNodeList[i].id;

				if (_dictNodesList.ContainsKey(__id) == false)
				{
					_dictNodesList.Add(__id, false);
				}
				
				GUILayout.BeginHorizontal();

				if (_dictNodesList[__id] == false) 
				{
					GUI.color = Color.grey;
				} 
				else 
				{
					GUI.color = Color.white;
				}
					
				if (SoundModule.instance.currentNodeList [i].VoxVoxSource.audioSource.clip == null)
				{
					if (GUILayout.Button ("Audio Node " + __id + ": null" ))
					{
						_dictNodesList [__id] = !_dictNodesList [__id];
					}
				} 
				else
				{

					if (GUILayout.Button ("Audio Node " + __id + ": " + SoundModule.instance.currentNodeList [i].VoxVoxSource.audioSource.clip.name))
					{
						_dictNodesList [__id] = !_dictNodesList [__id];
					}
				}

				GUILayout.FlexibleSpace();

				GUILayout.EndHorizontal();

				if (_dictNodesList[__id]) 
				{
					GUI.color = Color.white;

					DisplayTweenNodeData(SoundModule.instance.currentNodeList[i]);
				}
			}

			EditorGUILayout.EndScrollView();

		}

		/// <summary>
		/// Function that display the information of passed node.
		/// </summary>
		public void DisplayTweenNodeData(SoundNode __node)
		{
			if (__node.onFinishedPlaying != null)
			{
				GUILayout.Label ("\tUsedByClass: " + __node.onFinishedPlaying.Method.ReflectedType.FullName);
				GUILayout.Label ("\tMethod: " + __node.onFinishedPlaying.Method.Name);
			}

			GUILayout.BeginHorizontal ();
				GUILayout.Label ("\tAudioclip: ");
				EditorGUILayout.ObjectField (__node.VoxVoxSource.audioSource.clip, typeof(AudioClip), true);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
				GUILayout.Label ("\tAudioSource: ");
				EditorGUILayout.ObjectField (__node.VoxVoxSource.audioSource , typeof(AudioSource), true);
			GUILayout.EndHorizontal ();

			GUILayout.Label ("\tIsLoop:" + __node.VoxVoxSource.audioSource.loop);
		}
	}
}
