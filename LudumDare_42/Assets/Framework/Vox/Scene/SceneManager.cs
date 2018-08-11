#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxInternal;
using UnityEngine.SceneManagement;
using System;

namespace Vox
{
	#region Enumerators

	/// <summary>
	/// Type of method that will receive passed arguments. (START OR AWAKE)
	/// </summary>
	/// <remarks>
	/// Type of method that will receive passed arguments. (START OR AWAKE)
	/// </remarks> 
	public enum ReceiveArgsMethodType
	{
		NONE,
		AWAKE,
		START
	};

	/// <summary>
	/// Type of load mode used to intansciate scene. Same as unity default enum
	/// </summary>
	/// <remarks>
	/// Type of load mode used to intansciate scene. Same as unity default enum.
	/// 
	/// For more information check: <BR>
	/// - <A href="http://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.html"><STRONG>Unity Load Scene Mode</STRONG></A>.
	/// </remarks>
	/// 
	public enum LoadSceneMode
	{
		ADDITIVE,
		SINGLE
	};

	#endregion

	/// <summary>
	/// Class to manage all vox scenes. Load, unload, activate and other.
	/// </summary>
	/// <remarks>
	/// Class to manage all vox scenes. Load, unload, activate and others.
	/// Its use is <STRONG>demantory</STRONG> for the framework to work properly.
	/// 
	/// Example of how this is used instead of Unity scene manager is show below:
	/// 
	/// <code>
	///	public class LoadingScene : Vox.Scene 
	///	{
	///		[SerializeField] private Slider _loadingProgressBar;
	///
	///		public void LoadSceneAsync (string p_sceneName)
	///		{
	///			Action<float> __handleLoadProgressAction = delegate(float p_progressValue)
	///			{
	///				_loadingProgressBar.value = p_progressValue;
	///			};
	///
	///			Vox.SceneManager.LoadSceneAsync (p_sceneName, Vox.LoadSceneMode.ADDITIVE, true, __handleLoadProgressAction, delegate()
	///			{
	///				Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_OUT_WHITE, 0.5f, Vox.EaseType.CUBIC_OUT, delegate()
	///				{
	///					Vox.SceneManager.UnloadScene("Loading");
	///					Vox.SceneManager.ActivateHoldedScene(p_sceneName, false);
	///
	///					Vox.Timer.WaitSeconds (0.1f, delegate()
	///					{
	///						Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_IN_WHITE, 0.5f, Vox.EaseType.CUBIC_IN, null);
	///					});
	///				});
	///			});
	///		}
	/// 
	///  	public void LoadScene (string p_sceneName)
	/// 	{
	/// 		Vox.SceneManager.LoadScene (p_sceneName, Vox.LoadSceneMode.SINGLE, true, null, null, null, delegate()
	///			{
	/// 			// Finished loading the scene
	/// 			// Other scenes have been removed.
	/// 			// Awake and start of scenes will we be called by the framework
	/// 		}
	/// 	}
	///	} 
	/// </code>
	/// 
	/// </remarks>  
	public class SceneManager
	{
		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif
	
		private static Dictionary<string,AsyncOperation>_dictScenesHoldingActivation = new Dictionary<string,AsyncOperation>();

		/// <summary>
		/// Internal corroutine to make scene be set as active as soon as loaded.
		/// </summary>
		private static IEnumerator SetSceneAsMainCorroutine(string p_sceneName)
		{
			yield return new WaitForEndOfFrame();

			while(UnityEngine.SceneManagement.SceneManager.GetSceneByName(p_sceneName).isLoaded == false)
			{
				yield return 0;
			}

			SceneManager.SetSceneAsMain (p_sceneName);
		}

		/// <summary>
		/// Internal corroutine to make scene be set as active as soon as loaded.
		/// </summary>
		private static IEnumerator CheckSceneLoadedCorroutine(string p_sceneName, Action p_finishCallback)
		{
			yield return new WaitForEndOfFrame();

			while(UnityEngine.SceneManagement.SceneManager.GetSceneByName(p_sceneName).isLoaded == false)
			{
				yield return 0;
			}

			if (p_finishCallback != null)
				p_finishCallback ();
		}

		/// <summary>
		/// Handler loadscene assync call, calling back delegated methods.
		/// </summary>
		private static IEnumerator HandlerLoadAsyncCorroutine(AsyncOperation p_operation, string p_sceneName, bool p_holdSceneActivation, LoadSceneMode p_loadSceneMode, Action<float> p_callbackProgress, Action p_finishCallback)
		{
			if (p_holdSceneActivation) 
			{
				_dictScenesHoldingActivation.Add (p_sceneName, p_operation);
			}

			while (p_operation.progress < 0.89f) 
			{
				yield return new WaitForEndOfFrame ();

				if (p_callbackProgress != null)
				{
					p_callbackProgress (0.95f/p_operation.progress);
				}
			}

			if (p_holdSceneActivation == false)
			{
				if (p_callbackProgress != null)
					p_callbackProgress (1f);
			}

			if(p_finishCallback != null)
				p_finishCallback ();

			if (p_loadSceneMode == LoadSceneMode.SINGLE && p_holdSceneActivation == false)
			{
				for (int i = UnityEngine.SceneManagement.SceneManager.sceneCount - 1; i >= 0; i--) 
				{
					if (UnityEngine.SceneManagement.SceneManager.GetSceneAt (i).name != "VoxFrameworkScene" && UnityEngine.SceneManagement.SceneManager.GetSceneAt (i).name != p_sceneName) 
					{
						UnityEngine.SceneManagement.Scene __scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt (i);

						if (__scene.isLoaded)
						{
							if (SceneCore.instance.GetSceneByName (__scene.name) != null)
							{
								SceneCore.instance.GetSceneByName (__scene.name).OnUnloadedScene ();
								SceneCore.instance.RemoveScene (__scene.name);
							}

							UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(__scene.name);
						}
					}
				}
			}
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region Load Methods

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static void LoadScene(int p_sceneIndex)
		{
			string __sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt (p_sceneIndex).name;

			LoadScene(__sceneName, LoadSceneMode.SINGLE, true, ReceiveArgsMethodType.NONE, null, null);
		}

		public static void LoadScene(int p_sceneIndex, LoadSceneMode p_sceneMode)
		{
			string __sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt (p_sceneIndex).name;

			LoadScene(__sceneName, p_sceneMode, true, ReceiveArgsMethodType.NONE, null, null);
		}

		public static void LoadScene(int p_sceneIndex, LoadSceneMode p_sceneMode,  bool p_setAsActiveScene = true)
		{
			string __sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt (p_sceneIndex).name;

			LoadScene(__sceneName, p_sceneMode, p_setAsActiveScene, ReceiveArgsMethodType.NONE, null, null);
		}
			
		public static void LoadScene(int p_sceneIndex, LoadSceneMode p_sceneMode, bool p_setAsActiveScene = true, ReceiveArgsMethodType p_methodType = ReceiveArgsMethodType.NONE, object[] p_argsToSend = null, GameObject[] p_gameObjectsToSend = null, Action p_sceneFinishedLoadCallback = null)
		{
			string __sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt (p_sceneIndex).name;

			LoadScene(__sceneName, p_sceneMode, p_setAsActiveScene, p_methodType, p_argsToSend, p_gameObjectsToSend, p_sceneFinishedLoadCallback);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Load a unity scene. 
		/// </summary>
		/// <remarks>
		/// 
		/// <B><i></c>void LoadScene(string p_sceneName)</i></B>:
		/// 
		/// Load a unity scene. Obrigatory use instead of default <A href="https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html"><STRONG>Unity.SceneManagement</STRONG></A>.
		///
		/// <code>
		/// 
		/// void LoadSceneTest()
		/// {
		///		Vox.SceneManager.LoadScene("StartScene");
		/// }
		/// 
		/// </code>
		/// 
		/// <B><i></c>void LoadScene(string p_sceneName, Vox.LoadSceneMode p_sceneMode)</i></B>:
		/// 
		/// Single to unload all other scenes and additive to load with other loaded scenes.
		/// This will initialize any Vox.Scene scripts that may be in the scene, but it is supossed to be used even with scenes without the Vox.Scene.
		/// 
		/// <code>
		/// 
		/// void LoadSceneTest()
		/// {
		///		Vox.SceneManager.LoadScene("StartScene", LoadSceneMode.SINGLE); // Loads the scene "StartScene", removing any other scenes loaded before. 
		/// 	Vox.SceneManager.LoadScene("GameScene", LoadSceneMode.SINGLE); // Loads the scene "GameScene", removing the scene "StartScene".
		/// 	Vox.SceneManager.LoadScene("ExtraBackgroundScene", LoadSceneMode.ADDITIVE); // Load the scene "ExtraBackgroundScene", without removing the scene "GameScene".
		/// 	Vox.SceneManager.LoadScene("EnemiesScene", LoadSceneMode.ADDITIVE); // Load the scene "EnemiesScene", without removing the scene "GameScene" or the scene "ExtraBackgroundScene".
		/// }
		/// 
		/// </code>
		/// 
		/// <B><i></c>void LoadScene((string p_sceneName, Vox.LoadSceneMode p_sceneMode,  bool p_setAsActiveScene = true)</i></B>:
		/// 
		/// You can also set the scene to start as active, for more information check: Vox.SceneManager.SetSceneAsMain;
		/// 
		/// <code>
		/// 
		/// void LoadSceneTest()
		/// {
		///		Vox.SceneManager.LoadScene("StartScene", LoadSceneMode.SINGLE); // Loads the scene "StartScene", removing any other scenes loaded before. 
		/// 	Vox.SceneManager.LoadScene("GameScene", LoadSceneMode.SINGLE, true); // Loads the scene "GameScene", removing the scene "StartScene".
		/// 	Vox.SceneManager.LoadScene("ExtraBackgroundScene", LoadSceneMode.ADDITIVE, false); // Load the scene "ExtraBackgroundScene", without removing the scene "GameScene".
		/// 	Vox.SceneManager.LoadScene("EnemiesScene", LoadSceneMode.ADDITIVE, false); // Load the scene "EnemiesScene", without removing the scene "GameScene" or the scene "ExtraBackgroundScene".
		/// 	
		/// 	// The scene "GameScene" will remain being the active scene even if the other scenes are loaded after.
		/// }
		/// 
		/// </code> 
		/// 
		/// <B><i>void LoadScene(string p_sceneName, Vox.LoadSceneMode p_sceneMode, bool p_setAsActiveScene = true, Vox.ReceiveArgsMethodType p_methodType = ReceiveArgsMethodType.NONE, object[] p_argsToSend = null, GameObject[] p_gameObjectsToSend = null, Action p_sceneFinishedLoadCallback = null)</i></B>:
		/// 
		/// You can pass any types of informations via object arrays or also send gameObjects arrays to either the Vox.Scene.SceneAwake or Vox.Scene.SceneStart functions of the scenes depending on the Vox.ReceiveArgsMethodType parameter.
		///  
		/// <code>
		/// 
		/// void LoadSceneTest()
		/// {
		/// 	string __startState = "Paused";
		/// 
		/// 	object[] __objectsToSend = new object[1];
		/// 	__objectsToSend[0] = __startState;
		/// 
		/// 	GameObject __emptyObject = new GameObject();
		/// 	__emptyObject.name = "PreviousName";
		/// 
		/// 	GameObject[] __gameObjectsToSend = new GameObject[1];
		/// 	__gameObjectsToSend[0] = __emptyObject; 
		/// 
		///		Vox.SceneManager.LoadScene("Game", LoadSceneMode.SINGLE, true, ReceiveArgsMethodType.AWAKE, __objectsToSend, __gameObjectsToSend, delegate()
		/// 	{
		/// 		//Finished loading scene "Game".
		/// 	}
		/// }
		/// 
		/// </code>
		/// 
		/// <B><i></c>void LoadScene(string p_sceneName)</i></B>:
		/// 
		/// You can also use buid scene index for loading scenes.
		///
		/// <code>
		/// 
		/// void LoadSceneTest()
		/// {
		///		Vox.SceneManager.LoadScene(0);
		/// }
		/// 
		/// </code>
		/// 
		/// <B><i></c>void LoadScene(int p_sceneIndex, Vox.LoadSceneMode p_sceneMode)</i></B>:
		/// 
		/// <code>
		/// 
		/// void LoadSceneTest()
		/// {
		///		Vox.SceneManager.LoadScene(2, LoadSceneMode.SINGLE); // Loads the scene 2, removing any other scenes loaded before. 
		/// 	Vox.SceneManager.LoadScene(0, LoadSceneMode.SINGLE); // Loads the scene 0, removing the scene 2.
		/// 	Vox.SceneManager.LoadScene(1, LoadSceneMode.ADDITIVE); // Load the scene 1, without removing the scene 0.
		/// 	Vox.SceneManager.LoadScene(2, LoadSceneMode.ADDITIVE); // Load the scene 2, without removing the scene 0 or the scene 1.
		/// }
		/// 
		/// </code>
		///
		/// <B><i></c>void LoadScene(int p_sceneIndex, Vox.LoadSceneMode p_sceneMode,  bool p_setAsActiveScene = true)</i></B>:
		/// 		 
		/// <code>
		/// 
		/// void LoadSceneTest()
		/// {
		///		Vox.SceneManager.LoadScene(2, LoadSceneMode.SINGLE); // Loads the scene 2, removing any other scenes loaded before. 
		/// 	Vox.SceneManager.LoadScene(0, LoadSceneMode.SINGLE, true); // Loads the scene 0, removing the scene 2.
		/// 	Vox.SceneManager.LoadScene(1, LoadSceneMode.ADDITIVE, false); // Load the scene 1, without removing the scene 0.
		/// 	Vox.SceneManager.LoadScene(2, LoadSceneMode.ADDITIVE, false); // Load the scene 2, without removing the scene 0 or the scene 1.
		/// 	
		/// 	// The scene 0 will remain being the active scene even if the other scenes are loaded after.
		/// }
		/// 
		/// </code>
		///
		/// <B><i>void LoadScene(int p_sceneIndex, Vox.LoadSceneMode p_sceneMode, bool p_setAsActiveScene = true, Vox.ReceiveArgsMethodType p_methodType = ReceiveArgsMethodType.NONE, object[] p_argsToSend = null, GameObject[] p_gameObjectsToSend = null, Action p_sceneFinishedLoadCallback = null)</i></B>:
		/// 		 
		/// <code>
		/// 
		/// void LoadSceneTest()
		/// {
		/// 	string __startState = "Paused";
		/// 
		/// 	object[] __objectsToSend = new object[1];
		/// 	__objectsToSend[0] = __startState;
		/// 
		/// 	GameObject __emptyObject = new GameObject();
		/// 	__emptyObject.name = "PreviousName";
		/// 
		/// 	GameObject[] __gameObjectsToSend = new GameObject[1];
		/// 	__gameObjectsToSend[0] = __emptyObject; 
		/// 
		///		Vox.SceneManager.LoadScene(2, LoadSceneMode.SINGLE, true, ReceiveArgsMethodType.AWAKE, __objectsToSend, __gameObjectsToSend, delegate()
		/// 	{
		/// 		//Finished loading scene 2.
		/// 	}
		/// }
		/// 
		/// </code>   
		/// 
		/// </remarks>
		public static void LoadScene(string p_sceneName)
		{
			LoadScene(p_sceneName, LoadSceneMode.SINGLE, true, ReceiveArgsMethodType.NONE, null, null);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static void LoadScene(string p_sceneName, LoadSceneMode p_sceneMode)
		{
			LoadScene(p_sceneName, p_sceneMode, true, ReceiveArgsMethodType.NONE, null, null);
		}


		public static void LoadScene(string p_sceneName, LoadSceneMode p_sceneMode,  bool p_setAsActiveScene)
		{
			LoadScene(p_sceneName, p_sceneMode, p_setAsActiveScene, ReceiveArgsMethodType.NONE, null, null);
		}
			
		public static void LoadScene(string p_sceneName, LoadSceneMode p_sceneMode, bool p_setAsActiveScene, ReceiveArgsMethodType p_methodType, object[] p_argsToSend = null, GameObject[] p_gameObjectsToSend = null, Action p_sceneFinishedLoadCallback = null)
		{
			if (p_setAsActiveScene)
			{
				GameCore.instance.StartCoroutine( SetSceneAsMainCorroutine( p_sceneName ) );
			}

			GameCore.instance.StartCoroutine(CheckSceneLoadedCorroutine (p_sceneName, p_sceneFinishedLoadCallback));

			if (p_methodType != ReceiveArgsMethodType.NONE) 
			{
				SceneDataParams __params = new SceneDataParams ();
				__params.sceneName = p_sceneName;
				__params.receiveMethodType = p_methodType;
				__params.argsToSend = p_argsToSend;
				__params.gameObjectsToSend = p_gameObjectsToSend;
				SceneCore.instance.AddSceneParams (__params);
			}

			switch (p_sceneMode) 
			{
			case LoadSceneMode.ADDITIVE:
				UnityEngine.SceneManagement.SceneManager.LoadScene(p_sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);

				if (p_gameObjectsToSend != null) 
				{
					for (int i = 0; i < p_gameObjectsToSend.Length; i++) 
					{
						p_gameObjectsToSend [i].transform.SetParent (null);
						UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (p_gameObjectsToSend [i], UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName));
					}
				}
				break;

			case LoadSceneMode.SINGLE:

				if (UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName).isLoaded) 
				{
					if (SceneCore.instance.GetSceneByName (p_sceneName) != null)
					{
						SceneCore.instance.GetSceneByName (p_sceneName).OnUnloadedScene ();
						SceneCore.instance.RemoveScene (p_sceneName);
					}

					UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync (p_sceneName);
				}

				UnityEngine.SceneManagement.SceneManager.LoadScene(p_sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);

				if (p_gameObjectsToSend != null) 
				{
					for (int i = 0; i < p_gameObjectsToSend.Length; i++) 
					{
						p_gameObjectsToSend [i].transform.SetParent (null);
						UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (p_gameObjectsToSend [i], UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName));
					}
				}

				for (int i = UnityEngine.SceneManagement.SceneManager.sceneCount - 1; i >= 0 ; i--)
				{
					if (UnityEngine.SceneManagement.SceneManager.GetSceneAt (i).name != "VoxFrameworkScene" && UnityEngine.SceneManagement.SceneManager.GetSceneAt (i).name != p_sceneName)
					{
						UnityEngine.SceneManagement.Scene __scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt (i);

						if (__scene.isLoaded)
						{
							if (SceneCore.instance.GetSceneByName (__scene.name) != null)
							{
								SceneCore.instance.GetSceneByName (__scene.name).OnUnloadedScene ();
								SceneCore.instance.RemoveScene (__scene.name);
							}

							UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync (__scene.name);
						}
					}
				}
				break;
			default:
				break;
			}
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region Async Load Methods
	
		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static void LoadSceneAsync(int p_sceneIndex, LoadSceneMode p_sceneMode, Action<float> p_callbackProgress = null, Action p_callbackFinish = null)
		{
			string __sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt (p_sceneIndex).name;
			LoadSceneAsync (__sceneName, p_sceneMode, false, true, ReceiveArgsMethodType.NONE,  null, null, p_callbackProgress, p_callbackFinish);
		}
			
		public static void LoadSceneAsync(int p_sceneIndex, LoadSceneMode p_sceneMode, bool p_holdSceneActivation, Action<float> p_callbackProgress, Action p_callbackFinish)
		{
			string __sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt (p_sceneIndex).name;
			LoadSceneAsync (__sceneName, p_sceneMode, p_holdSceneActivation, true, ReceiveArgsMethodType.NONE,  null, null, p_callbackProgress, p_callbackFinish);
		}
			
		public static void LoadSceneAsync(int p_sceneIndex, LoadSceneMode p_sceneMode, bool p_holdSceneActivation,  bool p_setAsActiveScene, Action<float> p_callbackProgress, Action p_callbackFinish)
		{
			string __sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt (p_sceneIndex).name;
			LoadSceneAsync (__sceneName, p_sceneMode, p_holdSceneActivation, p_setAsActiveScene, ReceiveArgsMethodType.NONE, null, null, p_callbackProgress, p_callbackFinish);
		}
			
		public static void LoadSceneAsync(int p_sceneIndex, LoadSceneMode p_sceneMode, bool p_holdSceneActivation, bool p_setAsActiveScene, ReceiveArgsMethodType p_methodType, object[] p_argsToSend = null, GameObject[] p_gameObjectsToSend = null, Action<float> p_callbackProgress = null, Action p_finishCallback = null)
		{
			string __sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt (p_sceneIndex).name;
			LoadSceneAsync (__sceneName, p_sceneMode, p_holdSceneActivation, p_setAsActiveScene, p_methodType, p_argsToSend, p_gameObjectsToSend, p_callbackProgress, p_finishCallback);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Load asynchronously a unity scene. 
		/// </summary>
		/// <remarks>
		/// 
		/// Load asynchronously a unity scene. Obrigatory use instead of default unity SceneManager.
		/// Single to unload all other scenes and additive to load with other loaded scenes.
		/// There is two callbacks available in this function, the callbackProgress, which return the percentage from 0 to 1 of the state of the load screen.
		/// And the finishCallback which tells that the loading has finished.
		/// 
		/// You can preload a scene and finish loading it only later via Vox.SceneManager.ActivateHoldedScene.
		/// You can also set the scene to start active as the main scene and send parameters just as in Vox.SceneManager.LoadScene.
		/// 
		/// <code>
		/// 
		///	public class LoadingScene : Vox.Scene 
		///	{
		///		[SerializeField]
		///		private Slider _loadingProgressBar;
		///
		///		public void LoadScene (string p_sceneName)
		///		{
		///			Action<float> __handleLoadProgressAction = delegate(float p_progressValue)
		///			{
		///				_loadingProgressBar.value = p_progressValue;
		///			};
		///
		///			Vox.Timer.WaitSeconds (0.5f, delegate()
		///			{
		///				Vox.SceneManager.LoadSceneAsync (p_sceneName, Vox.LoadSceneMode.ADDITIVE, true, __handleLoadProgressAction, delegate()
		///				{
		///					Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_OUT_WHITE, 0.5f, Vox.EaseType.CUBIC_OUT, delegate()
		///					{
		///						Vox.SceneManager.UnloadScene("Loading");
		///						Vox.SceneManager.ActivateHoldedScene(p_sceneName, false);
		///
		///						Vox.Timer.WaitSeconds (0.1f, delegate()
		///						{
		///							Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_IN_WHITE, 0.5f, Vox.EaseType.CUBIC_IN, null);
		///						});
		///					});
		///				});
		///			});
		///		}
		///	}
		/// 
		/// </code>
		/// 
		/// <STRONG>Overrides Availables:</STRONG>
		/// 
		/// <code>
		/// 
		/// void LoadSceneAsync(string p_name, LoadSceneMode p_sceneMode, Action<float> p_callbackProgress = null, Action p_callbackFinish = null)
		/// 
		/// void LoadSceneAsync(string p_name, LoadSceneMode p_sceneMode, bool p_holdSceneActivation, Action<float> p_callbackProgress, Action p_callbackFinish)
		/// 
		/// void LoadSceneAsync(string p_name, LoadSceneMode p_sceneMode, bool p_holdSceneActivation, bool p_setAsActiveScene, Action<float> p_callbackProgress, Action p_callbackFinish)
		/// 
		/// void LoadSceneAsync(string p_name, LoadSceneMode p_sceneMode,  bool p_holdSceneActivation, bool p_setAsActiveScene, ReceiveArgsMethodType p_methodType, object[] p_argsToSend = null, GameObject[] p_gameObjectsToSend = null, Action<float> p_callbackProgress = null, Action p_finishCallback = null)
		///
		/// void LoadSceneAsync(int p_sceneIndex, LoadSceneMode p_sceneMode, Action<float> p_callbackProgress = null, Action p_callbackFinish = null)
		/// 
		/// void LoadSceneAsync(int p_sceneIndex, LoadSceneMode p_sceneMode, bool p_holdSceneActivation, Action<float> p_callbackProgress, Action p_callbackFinish)
		/// 
		/// void LoadSceneAsync(int p_sceneIndex, LoadSceneMode p_sceneMode, bool p_holdSceneActivation,  bool p_setAsActiveScene, Action<float> p_callbackProgress, Action p_callbackFinish)
		/// 
		/// void LoadSceneAsync(int p_sceneIndex, LoadSceneMode p_sceneMode, bool p_holdSceneActivation, bool p_setAsActiveScene, ReceiveArgsMethodType p_methodType, object[] p_argsToSend = null, GameObject[] p_gameObjectsToSend = null, Action<float> p_callbackProgress = null, Action p_finishCallback = null))
		///
		/// </code>
		/// 
		/// </remarks>
		public static void LoadSceneAsync(string p_name, LoadSceneMode p_sceneMode, Action<float> p_callbackProgress = null, Action p_callbackFinish = null)
		{
			LoadSceneAsync (p_name, p_sceneMode, false, true, ReceiveArgsMethodType.NONE,  null, null, p_callbackProgress, p_callbackFinish);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static void LoadSceneAsync(string p_name, LoadSceneMode p_sceneMode, bool p_holdSceneActivation, Action<float> p_callbackProgress, Action p_callbackFinish)
		{
			LoadSceneAsync (p_name, p_sceneMode, p_holdSceneActivation, true, ReceiveArgsMethodType.NONE, null, null, p_callbackProgress, p_callbackFinish);
		}
			
		public static void LoadSceneAsync(string p_name, LoadSceneMode p_sceneMode, bool p_holdSceneActivation, bool p_setAsActiveScene, Action<float> p_callbackProgress, Action p_callbackFinish)
		{
			LoadSceneAsync (p_name, p_sceneMode, p_holdSceneActivation, p_setAsActiveScene, ReceiveArgsMethodType.NONE, null, null, p_callbackProgress, p_callbackFinish );
		}
			
		public static void LoadSceneAsync(string p_name, LoadSceneMode p_sceneMode,  bool p_holdSceneActivation, bool p_setAsActiveScene, ReceiveArgsMethodType p_methodType, object[] p_argsToSend = null, GameObject[] p_gameObjectsToSend = null, Action<float> p_callbackProgress = null, Action p_finishCallback = null)
		{
			if(p_setAsActiveScene)
				GameCore.instance.StartCoroutine(SetSceneAsMainCorroutine (p_name));
			
			if (p_methodType != ReceiveArgsMethodType.NONE) 
			{
				SceneDataParams __params = new SceneDataParams ();
				__params.sceneName = p_name;
				__params.receiveMethodType = p_methodType;
				__params.argsToSend = p_argsToSend;
				__params.gameObjectsToSend = p_gameObjectsToSend;
				SceneCore.instance.AddSceneParams (__params);
			}

			AsyncOperation __operation = null;

			switch (p_sceneMode) 
			{
				case LoadSceneMode.ADDITIVE:
					__operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (p_name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
					__operation.allowSceneActivation = !p_holdSceneActivation;

					GameCore.instance.StartCoroutine (HandlerLoadAsyncCorroutine (__operation, p_name, p_holdSceneActivation, p_sceneMode, p_callbackProgress, p_finishCallback));

					if (p_gameObjectsToSend != null) 
					{
						for (int i = 0; i < p_gameObjectsToSend.Length; i++) 
						{
							p_gameObjectsToSend [i].transform.SetParent (null);
							UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (p_gameObjectsToSend [i], UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_name));
						}
					}
					break;

				case LoadSceneMode.SINGLE:
						__operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (p_name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
						__operation.allowSceneActivation = !p_holdSceneActivation;

						if (UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_name).isLoaded) 
						{
							if (SceneCore.instance.GetSceneByName (p_name) != null)
							{
								SceneCore.instance.GetSceneByName (p_name).OnUnloadedScene ();
								SceneCore.instance.RemoveScene (p_name);
							}

							UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync (p_name);
						}

						GameCore.instance.StartCoroutine (HandlerLoadAsyncCorroutine  (__operation, p_name, p_holdSceneActivation, p_sceneMode, p_callbackProgress, p_finishCallback));

						if (p_gameObjectsToSend != null) 
						{
							for (int i = 0; i < p_gameObjectsToSend.Length; i++) 
							{
								p_gameObjectsToSend [i].transform.SetParent (null);
								UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (p_gameObjectsToSend [i], UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_name));
							}
						}
						break;

					default:
						break;
			}
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		/// <summary>
		/// Acivate a scene preloaded via an assync load function.
		/// </summary>
		/// <remarks>
		/// Acivate a scene preloaded via an assync load function. Usefull for preloading a scene while performing an action and only finishing the load when desired.
		/// <code>
		/// 
		///	public class LoadingScene : Vox.Scene 
		///	{
		///		[SerializeField]
		///		private Slider _loadingProgressBar;
		///
		///		public void LoadScene (string p_sceneName)
		///		{
		///			Action<float> __handleLoadProgressAction = delegate(float p_progressValue)
		///			{
		///				_loadingProgressBar.value = p_progressValue;
		///			};
		///
		///			Vox.Timer.WaitSeconds (0.5f, delegate()
		///			{
		///				Vox.SceneManager.LoadSceneAsync (p_sceneName, Vox.LoadSceneMode.ADDITIVE, true, __handleLoadProgressAction, delegate()
		///				{
		///					Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_OUT_WHITE, 0.5f, Vox.EaseType.CUBIC_OUT, delegate()
		///					{
		///						Vox.SceneManager.UnloadScene("Loading");
		///						Vox.SceneManager.ActivateHoldedScene(p_sceneName, false);
		///
		///						Vox.Timer.WaitSeconds (0.1f, delegate()
		///						{
		///							Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_IN_WHITE, 0.5f, Vox.EaseType.CUBIC_IN, null);
		///						});
		///					});
		///				});
		///			});
		///		}
		///	}
		/// 
		/// </code>
		///  
		/// </remarks>
		public static void ActivateHoldedScene(string p_sceneName, LoadSceneMode p_loadSceneMode)
		{
			if (_dictScenesHoldingActivation.ContainsKey (p_sceneName)) 
			{
				_dictScenesHoldingActivation [p_sceneName].allowSceneActivation = true;
				_dictScenesHoldingActivation.Remove (p_sceneName);

				if (p_loadSceneMode == LoadSceneMode.SINGLE) 
				{
					for (int i = UnityEngine.SceneManagement.SceneManager.sceneCount - 1; i >= 0; i--) 
					{
						if (UnityEngine.SceneManagement.SceneManager.GetSceneAt (i).name != "VoxFrameworkScene" && UnityEngine.SceneManagement.SceneManager.GetSceneAt (i).name != p_sceneName)
						{
							UnityEngine.SceneManagement.Scene __scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt (i);

							if (__scene.isLoaded) 
							{
								if (SceneCore.instance.GetSceneByName (__scene.name) != null) 
								{
									SceneCore.instance.GetSceneByName (__scene.name).OnUnloadedScene ();
									SceneCore.instance.RemoveScene (__scene.name);
								}

								UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync (__scene.name);
							}
						}
					}
				}
			}
		}
		#endregion

		#region Utilites
		/// <summary>
		/// Send a GameObject to a loaded scene.
		/// </summary>
		/// <remarks>
		/// Send a gameobject to the root of the scene with the passed name. The scene will receive a callback with the object in the Vox.Scene.onReceivedGameObject action.
		/// </remarks>
		public static void SendGameObjectToLoadedScene(GameObject p_gameObjectToSend, string p_sceneName)
		{
			if (UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName).isLoaded) 
			{
				p_gameObjectToSend.transform.SetParent (null);
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (p_gameObjectToSend, UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName));

				if (SceneCore.instance.GetSceneByName (p_sceneName) != null)
				{
					if (SceneCore.instance.GetSceneByName (p_sceneName).onReceivedGameObject != null)
						SceneCore.instance.GetSceneByName (p_sceneName).onReceivedGameObject (p_gameObjectToSend); 
				}
			}
		}

		/// <summary>
		/// Send an array of GameObjects to a loaded scene.
		/// </summary>
		/// <remarks>
		/// Send an array of GameObjects to the root of the scene with the passed name. The scene will receive a callback with the object in the Vox.Scene.onReceivedGameObjectArray action.
		/// </remarks>
		public static void SendGameObjectsToLoadedScene(GameObject[] p_gameObjectsToSend, string p_sceneName)
		{
			if (UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName).isLoaded) 
			{
				for (int i = 0; i < p_gameObjectsToSend.Length; i++) 
				{
					p_gameObjectsToSend [i].transform.SetParent (null);
					UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (p_gameObjectsToSend [i], UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName));
				}

				if (SceneCore.instance.GetSceneByName (p_sceneName) != null)
				{
					if (SceneCore.instance.GetSceneByName (p_sceneName).onReceivedGameObjectArray != null)
						SceneCore.instance.GetSceneByName (p_sceneName).onReceivedGameObjectArray (p_gameObjectsToSend); 
				}
			}
		}

		/// <summary>
		/// Set scene as the active scene in unity. (The main scene)
		/// </summary>
		/// <remarks>
		/// Set scene as active in unity. Meaning that instantiated objects will be created at this scene and global lighting will use scene configurations.
		/// 
		/// If you have multiple scenes in your game and you want to make sure that you instantiated objects go to a specific one, define it as the main one with that function.
		/// 
		/// </remarks>   
		public static void SetSceneAsMain(string p_sceneName)
		{
			UnityEngine.SceneManagement.SceneManager.SetActiveScene (UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName));
		}
		#endregion

		#region Unload
		/// <summary>
		/// Unload a scene if it is loaded.
		/// </summary>
		/// <remarks>
		/// Unload a scene if it is loaded. Before the scene is unloaded, if it has a Vox.Scene script in it, the function Vox.Scene.OnUnloadedScene() will be called.
		/// Useful for managing the unload of scenes when the user is controlling scenes additively.
		/// </remarks>
		public static void UnloadScene(string p_sceneName)
		{
			if (UnityEngine.SceneManagement.SceneManager.GetSceneByName (p_sceneName).isLoaded) 
			{
				if (SceneCore.instance.GetSceneByName (p_sceneName) != null) 
				{
					SceneCore.instance.GetSceneByName (p_sceneName).OnUnloadedScene ();
					SceneCore.instance.RemoveScene (p_sceneName);
				}

				UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync (p_sceneName);
			} 
			else 
			{
				Debug.Log ("Scene is not loaded");
			}
		}
		#endregion
	}
}