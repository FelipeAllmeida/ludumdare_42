using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vox;
using VoxInternal;

public class AboutWindow : EditorWindow
{
	public enum NotesType
	{
		IMPROVEMENTS,
		CHANGES,
		FIXES,
		ISSUES
	};

	private const string _frameworkVersion = "Version 2.0.1";
	private const string _unityVersion = "5.3.5f1";

	private static AboutWindow _window;

	private static NotesType _noteSelectedType;

	[MenuItem("Vox/About")]
	/// <summary>
	/// Initialize the unity editor window.
	/// </summary>
	static private void Initialize () 
	{
		if(_window == null)
			GetWindow ();

		_window.titleContent.text = "About";
		_window.titleContent.image = (Texture)AssetDatabase.LoadAssetAtPath ("Assets/VoxFramework/Resources/VoxEditorData/VoxLogo32.png", typeof(Texture));
		_window.minSize = new Vector2 (320, 320);
	}

	/// <summary>
	/// Unity function to get editor window reference.
	/// </summary>
	static private void GetWindow()
	{
		_window = (AboutWindow)EditorWindow.GetWindowWithRect(typeof(AboutWindow), new Rect(380, 100, 1000, 320));
		_window.ShowPopup ();
		_window.minSize = new Vector2 (320, 320);
	}

	private void OnGUI()
	{
		if (_window == null)
			GetWindow ();

		ShowVersionInfo ();
		SelectedNotesInfoSection ();

		switch (_noteSelectedType) 
		{
		case NotesType.CHANGES:
			ShowChangesNotes ();
				break;
		case NotesType.IMPROVEMENTS:
			ShowImprovementsNotes ();
				break;
		case NotesType.FIXES:
			ShowFixesNotes ();
				break;
		case NotesType.ISSUES:
			ShowIssuesNotes ();
				break;
		default:
				break;
		}
	}

	private void ShowVersionInfo()
	{
		GUILayout.BeginHorizontal ();

		GUILayout.Label ("Vox Framework - " + _frameworkVersion, EditorStyles.boldLabel);
		GUILayout.FlexibleSpace ();
		GUILayout.Label ("Made in Unity " + _unityVersion, EditorStyles.label);

		GUILayout.EndHorizontal ();

		EditorGUILayout.Space();

	}

	private void SelectedNotesInfoSection()
	{
		GUILayout.BeginHorizontal ();

		for (int i = 0; i < Enum.GetValues (typeof(NotesType)).Length; i++) 
		{
			NotesType __type = ((NotesType)Enum.GetValues (typeof(NotesType)).GetValue (i));

			if (__type == _noteSelectedType) 
			{
				GUI.color = Color.cyan;
			}
			else
			{
				GUI.color = Color.white;
			}

			if(GUILayout.Button(__type.ToString(),  EditorStyles.miniButton, GUILayout.MaxWidth(100)))
			{
				_noteSelectedType = __type;
			}
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal ();
		EditorGUILayout.Space();
	}

	private void ShowImprovementsNotes()
	{
		string __features = "";
		__features = "- None.";
		__features += "\n\n";

		GUILayout.TextArea (__features, GUILayout.MinHeight(250),GUILayout.MaxWidth(1000));
	}

	private void ShowChangesNotes()
	{
		string __changes = "";
		__changes = "- The State functions 'StateOnEnable', 'StateOnDisable' and 'StateInitialize' were changed to virtual instead of abstract. Not requiring implementation if not used. ";
		__changes += "\n\n";

		GUILayout.TextArea (__changes, GUILayout.MinHeight(250),GUILayout.MaxWidth(1000));
	}

	private void ShowFixesNotes()
	{
		string __fixes = "";
		__fixes += "- Fixed a memory leak with the SoundCore where AudioSources would be added whenever a Sound function was called. Now it stores buffered AudioSources that are reused when a sound have finished";
		__fixes += "\n\n- Fixed a bug where the States inside the StateMachine would not be found or initialized if the objects were not active.";
		__fixes += "\n\n- Fixed a bug where user would not be able to reload a scene and would load another scene in addition instead.";
		__fixes += "\n\n- Cleaned VoxEditor so it not comes in the project with data in it already via the Package.";
		__fixes += "\n\n- VoxTests internal folder was upload alongside the package by mistake, now it contains contains only the VoxFramework official content.";

		GUILayout.TextArea (__fixes, GUILayout.MinHeight(250),GUILayout.MaxWidth(1000));
	}

	private void ShowIssuesNotes()
	{
		string __issues = "";
		__issues = "- AudioMixer is still a little complicated to work with, futures adjustments will guarantee a better audio organization with less scripting and more editor to change parameters and options.";
		__issues += "\n\n";

		GUILayout.TextArea (__issues, GUILayout.MinHeight(250),GUILayout.MaxWidth(1000));
	}
}
