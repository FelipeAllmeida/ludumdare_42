#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VoxInternal;

namespace Vox
{
	/// <summary>
	/// The Vox.Scene is where all logic of a scene should happen.
	/// </summary>
	/// <remarks>
	/// The Vox.Scene is where all logic of a scene happens.
	/// Follows the logic of default unity scrips, allowing you to use equivalents of all default functions:
	/// 
	/// SceneAwake = Awake; <BR>
	/// SceneStart = Start; <BR>
	/// SceneUpdate = Update; <BR>
	/// SceneFixedUpdate = FixedUpdate; <BR>
	/// SceneLateUpdate = LateUpdate; <BR>
	/// SceneOnGUI = OnGUI; <BR>
	/// SceneOnApplicationFocused = OnApplicationFocused; <BR>
	/// SceneOnApplicationPaused = OnApplicationPaused; <BR>
	/// SceneOnGameQuit = OnQuit; <BR>
	///
	/// Example of a class that inherits from Scene:
	/// 
	/// <code>
	/// 
	/// using UnityEngine;
	/// using System.Collections;
	/// using Vox;
	/// 
	/// public class GameScene : Scene
	/// {
	/// 	[SerializeField] private GameHUDStateMachine _HUDstateMachine;
	/// 
	/// 	[SerializeField] private Gameplay _gamePlay;
	/// 
	/// 	private float _currentScore = 0;
	/// 
	/// 	public override void SceneAwake (object[] p_argsToReceive, GameObject[] p_objectsToReceive)
	/// 	{
	/// 		base.SceneAwake (p_argsToReceive, p_objectsToReceive);
	/// 		InitializeHUDStateMachine ();
	/// 	}
	/// 
	/// 	private void InitializeHUDStateMachine()
	/// 	{
	/// 		_HUDstateMachine.StateMachineInitialize ();
	/// 
	/// 		_HUDstateMachine.InitializeStatesEvents ();
	/// 
	/// 		_HUDstateMachine.onFinishedStartCountdown += delegate() 
	/// 		{
	/// 			_gamePlay.StartGame();
	/// 		};
	/// 
	/// 	}
	/// 
	/// 	public override void SceneUpdate ()
	/// 	{
	/// 		base.SceneUpdate ();
	/// 
	/// 		_gamePlay.UpdateGamePlay ();
	/// 	}
	/// }
	/// </code>
	/// 
	/// </remarks>
	public abstract class Scene : MonoBehaviour
	{
		#region Private Internal Only

		#region Awake

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		/// <summary>
		/// Unity default awake. Used to add scene to SceneCore
		/// </summary>
		protected void Awake()
		{
			SceneCore.instance.AddScene (this);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#endregion


		#region Public Actions
		/// <summary>
		/// Action called when an GameObjects is sent to this scene.
		/// </summary>
		/// <remarks>
		/// Action called when an GameObjects is sent to this scene, passing the reference of the object.
		/// </remarks>
		public Action<GameObject> onReceivedGameObject;
		/// <summary>
		/// Action called when an array of GameObjects is sent to this scene.
		/// </summary>
		/// <remarks>
		/// Action called when an array of GameObjects is sent to this scene, passing the array as reference.
		/// </remarks>
		public Action<GameObject[]>onReceivedGameObjectArray;

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Awake method of scene, can receive parameters and gameobjects.
		/// </summary>
		/// <remarks>
		/// Awake method of scene, can receive parameters and gameobjects from the Vox.LoadScene() or Vox.LoadSceneAsync();
		/// 
		/// Code example below, such as the awake may receive a string with the name of a scene to load, or not:
		/// 
		/// <code>
		/// 
		/// public override void SceneAwake (object[] p_argsToReceive, GameObject[] p_objectsToReceive)
		/// {
		///		if (p_argsToReceive != null) 
		///		{
		///			string __nextSceneToLoad = (string) p_argsToReceive [0];
		///
		///			LoadScene  (__nextSceneToLoad);
		///		}
		///		else
		///		{
		///			StartHomeSceneLoading ();
		///		}
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks> 
		public virtual void SceneAwake(object[] p_argsToReceive = null, GameObject[] p_objectsToReceive = null)
		{
			
		}

		/// <summary>
		/// Start method of scene, can receive parameters and gameobjects.
		/// </summary>
		/// <remarks>
		/// Start method of scene, can receive parameters and gameobjects from the Vox.LoadScene() or Vox.LoadSceneAsync();
		/// 
		/// Code example below, such as the awake may receive a string with the name of a scene to load, or not:
		/// 
		/// <code>
		/// 
		/// public override void SceneStart(object[] p_argsToReceive, GameObject[] p_objectsToReceive)
		/// {
		///		if (p_argsToReceive != null) 
		///		{
		///			string __nextSceneToLoad = (string) p_argsToReceive [0];
		///
		///			LoadScene  (__nextSceneToLoad);
		///		}
		///		else
		///		{
		///			StartHomeSceneLoading ();
		///		}
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks> 
		public virtual void SceneStart(object[] p_argsToReceive = null, GameObject[] p_objectsToReceive = null)
		{

		}

		/// <summary>
		/// Update method of scene.
		/// </summary>
		/// <remarks>
		/// Update method of scene, equivalent to Unity update, called internaly by the framework.
		/// </remarks>
		public virtual void SceneUpdate()
		{

		}

		/// <summary>
		/// Fixed Update method of scene.
		/// </summary>
		/// <remarks>
		/// Fixed Update method of scene, equivalent to Unity Fixed update, called internaly by the framework.
		/// </remarks>
		public virtual void SceneFixedUpdate()
		{

		}

		/// <summary>
		/// Late Update method of scene.
		/// </summary>
		/// <remarks>
		/// Late Update method of scene, equivalent to Unity Late update, called internaly by the framework.
		/// </remarks>
		public virtual void SceneLateUpdate()
		{

		}

		/// <summary>
		/// OnGUI method of scene.
		/// </summary>
		/// <remarks>
		/// OnGUI method of scene, equivalent to Unity  OnGUI, called internaly by the framework.
		/// </remarks>
		public virtual void SceneOnGUI()
		{

		}

		/// <summary>
		/// Function that gets called on scene is unloaded by the framework.
		/// </summary>
		/// <remarks>
		/// Function that gets called on scene is unloaded by the framework when user uses Vox.UnloadScene();
		/// </remarks> 
		public virtual void OnUnloadedScene()
		{

		}

		/// <summary>
		/// Called when application is returned from running on background.
		/// </summary>
		/// <remarks>
		/// Called when application is returned from running on background, usefull for mobiles devices to check when user maximizes the application.
		/// </remarks> 
		public virtual void SceneOnApplicationFocused(bool p_focused)
		{

		}

		/// <summary>
		/// Called when application is paused or sent to background.
		/// </summary>
		/// <remarks>
		/// Called when application is paused or sent to background, usefull for mobiles devices to check when user minimes the application.
		/// </remarks>  
		public virtual void SceneOnApplicationPaused(bool p_paused)
		{

		}

		/// <summary>
		/// Called when application is about to quit.
		/// </summary>
		/// <remarks>
		/// Called when application is about to quit. Usefull for delete data or save data if required.
		/// </remarks>   
		public virtual void SceneOnApplicationQuit()
		{

		}

		#endregion
	}
}