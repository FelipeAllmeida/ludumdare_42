using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vox;

namespace VoxInternal
{
	/// <summary>
	/// Class that stores data related to the inicialization of scenes.
	/// </summary>
	public class SceneDataParams
	{
		public string sceneName = "";
		public ReceiveArgsMethodType receiveMethodType = ReceiveArgsMethodType.NONE;
		public object[] argsToSend = null;
		public GameObject[] gameObjectsToSend = null;
	}

	/// <summary>
	/// The class that updates and manages internally all of the Scenes currently running in our game.
	/// </summary>
	public class SceneCore
	{
		#region Private Data
		private Dictionary<string, Scene> _dictScenes = new Dictionary<string, Scene> ();
		private List<SceneDataParams> _listDataParams = new List<SceneDataParams>();
		private List<string> _listNameOfScenesToStart = new List<string> ();
		#endregion

		#region Instance Initialization
		/// <summary>
		/// Create automatically the unique instance of this class.
		/// </summary>
		static private SceneCore _instance;
		static public SceneCore instance
		{
			get 
			{ 
				if (_instance == null)
				{
					_instance = InstanceInitialize();
				}

				return _instance;
			}
		}
		/// <summary>
		/// Handles the logic required in the inicialization of the instance.
		/// </summary>
		static private SceneCore InstanceInitialize()
		{
			SceneCore __instance = new SceneCore ();

			GameCore.instance.AddSceneCore(__instance);

			return __instance;
		}

		#endregion

		#region Scene Control
		/// <summary>
		/// Function to be called from the scene to send itself to this core and call its Awake function. Checking if any info from the scene manager has been passed to it.
		/// </summary>
		public void AddScene(Scene p_scene)
		{
            if (_dictScenes.ContainsKey(p_scene.gameObject.scene.name))
            {
                Debug.LogWarning("Scene already contained on scene dictionary, skiping");
                return;
            };

			_dictScenes.Add (p_scene.gameObject.scene.name, p_scene);
			_listNameOfScenesToStart.Add (p_scene.gameObject.scene.name);

			bool __hasAwakeDataParams = false;

			for (int i = _listDataParams.Count - 1; i >= 0 ; i--) 
			{
				if (_listDataParams [i].sceneName == p_scene.gameObject.scene.name) 
				{
					if (_listDataParams [i].receiveMethodType == ReceiveArgsMethodType.AWAKE)
					{
						__hasAwakeDataParams = true;
						_dictScenes [p_scene.gameObject.scene.name].SceneAwake (_listDataParams [i].argsToSend,_listDataParams [i].gameObjectsToSend);
						_listDataParams.RemoveAt (i);
						break;
					}
				}
			}

			if (__hasAwakeDataParams == false) 
			{
				p_scene.SceneAwake ();
			}
		}
		/// <summary>
		/// Function to be called from the scene manager to remove a scene from the game.
		/// </summary>
		public void RemoveScene(string p_sceneName)
		{
			GameCore.instance.StartCoroutine(RemoveSceneCorroutine(p_sceneName));
		}

		private IEnumerator RemoveSceneCorroutine(string p_sceneName)
		{
			yield return new WaitForEndOfFrame();
			_dictScenes.Remove (p_sceneName);
		}
		/// <summary>
		/// Function to be called from the scene manager to add data to a scene that will be loaded.
		/// </summary>
		public void AddSceneParams(SceneDataParams p_params)
		{
			_listDataParams.Add (p_params);
		}
		#endregion

		#region Scenes Messages
		/// <summary>
		/// Function that call the Start function of the scene. Checking if any info from the scene manager has been passed to it.
		/// Called internally from a control inside ScenesUpdate.
		/// </summary>
		private void ScenesStart()
		{
			for (int i = _listNameOfScenesToStart.Count - 1; i >= 0; i--) 
			{
				string __sceneName = _listNameOfScenesToStart [i];

				if (_dictScenes.ContainsKey(__sceneName)) 
				{
					bool _hasParams = false;

					for (int j =  _listDataParams.Count - 1; j >= 0; j--) 
					{
						if (_listDataParams[j].receiveMethodType == ReceiveArgsMethodType.START) 
						{
							if (_listDataParams [j].sceneName == __sceneName) 
							{
								_hasParams = true;
								_dictScenes [__sceneName].SceneStart (_listDataParams [j].argsToSend, _listDataParams [j].gameObjectsToSend);
								_listDataParams.RemoveAt (j);
								_listNameOfScenesToStart.RemoveAt (i);
								break;
							}
						}
					}

					if (_hasParams == false) 
					{
						_dictScenes [__sceneName].SceneStart ();
						_listNameOfScenesToStart.RemoveAt (i);
					}
				}
			}
		}
		/// <summary>
		/// Function that calls the Update function of the scene. Also calling the start of the scene if not called yet.
		/// </summary>
		public void ScenesUpdate()
		{
			if (_listNameOfScenesToStart.Count > 0)
			{
				ScenesStart ();
			}

			foreach(KeyValuePair<string, Scene>__scene in _dictScenes)
			{
				__scene.Value.SceneUpdate ();
			}
		}
		/// <summary>
		/// Function that calls the Fixed Update function of the scene.
		/// </summary>
		public void ScenesFixedUpdate()
		{
            if (_listNameOfScenesToStart.Count > 0)
            {
                ScenesStart();
            }

            foreach (KeyValuePair<string, Scene>__scene in _dictScenes)
			{
				__scene.Value.SceneFixedUpdate ();
			}
		}
		/// <summary>
		/// Function that calls the Late Update function of the scene.
		/// </summary>
		public void ScenesLateUpdate()
		{
            if (_listNameOfScenesToStart.Count > 0)
            {
                ScenesStart();
            }

            foreach (KeyValuePair<string, Scene>__scene in _dictScenes)
			{
				__scene.Value.SceneLateUpdate ();
			}
		}
		/// <summary>
		/// Function that calls the GUI function implemented on the scene.
		/// </summary>
		public void ScenesOnGUI()
		{
			foreach(KeyValuePair<string, Scene>__scene in _dictScenes)
			{
				__scene.Value.SceneOnGUI ();
			}
		}
		/// <summary>
		/// Called when application is returned and coming from background, usefull for mobiles devices to check when user maximizes the application.
		/// </summary>
		public void ScenesOnApplicationFocused(bool p_focused)
		{
			foreach(KeyValuePair<string, Scene>__scene in _dictScenes)
			{
				__scene.Value.SceneOnApplicationFocused (p_focused);
			}
		}
		/// <summary>
		/// Called when application is paused or sent to background, usefull for mobiles devices to check when user minimes the application.
		/// </summary>
		public void ScenesOnApplicationPaused(bool p_paused)
		{
			foreach(KeyValuePair<string, Scene>__scene in _dictScenes)
			{
				__scene.Value.SceneOnApplicationFocused (p_paused);
			}
		}
		/// <summary>
		/// Called when application is about to quit. Usefull or delete data if required.
		/// </summary>
		public void ScenesOnApplicationQuit()
		{
			foreach(KeyValuePair<string, Scene>__scene in _dictScenes)
			{
				__scene.Value.SceneOnApplicationQuit ();
			}
		}
		/// <summary>
		/// Return a Vox Scene based on its name.
		/// </summary>
		public Scene GetSceneByName(string p_name)
		{
			if (_dictScenes.ContainsKey (p_name))
			{
				return _dictScenes [p_name];
			} 

			return null;
		}
		#endregion
	}


}
