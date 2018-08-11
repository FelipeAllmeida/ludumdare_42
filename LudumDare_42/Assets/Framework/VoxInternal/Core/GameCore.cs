using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Vox;

namespace VoxInternal
{
    /// <summary>
    /// The center of Vox Framework. Updates all other cores of the framework such as scene core and modules core.
    /// Similar to a main function in an standard cpp, this is our main for all our scripts to be called.
    /// </summary>
    public class GameCore : MonoBehaviour
    {
        #region Private Data
        private ModuleCore _moduleCore;
        private SceneCore _sceneCore;
        private ScreenCore _screenCore;

        private System.Threading.Thread _mainThread;
        #endregion

        #region Instance Initialization
        /// <summary>
        /// Create automatically the unique instance of this class.
        /// </summary>
        static private GameCore _instance;
        static public GameCore instance
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
        static private GameCore InstanceInitialize()
        {
            UnityEngine.SceneManagement.Scene __scene;

            __scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("VoxFrameworkScene");

            if (__scene.name != "VoxFrameworkScene")
            {
                if (Application.isPlaying)
                    __scene = UnityEngine.SceneManagement.SceneManager.CreateScene("VoxFrameworkScene");
                else
                    __scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            }

            GameObject __gameCore = new GameObject("VoxGameCore", typeof(GameCore));



            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(__gameCore, __scene);

            return __gameCore.GetComponent<GameCore>();
        }

        static private void CheckIfMuteAudio()
        {
            EditorSoundData __soundData = Resources.Load("VoxSoundData/VoxSoundSavedData") as EditorSoundData;

            if (__soundData != null)
            {
                if (__soundData.enableSoundInEditor == false)
                    AudioListener.volume = 0;
            }
        }
        #endregion

        #region Add Cores
        /// <summary>
        /// Function that checks if the fps debug screen should be activated or not.
        /// </summary>
        public void CheckIfDebugFPSEnabled()
        {
            //FieldDataEnabledState __fpsState = EditorDataManager.isFPSInBuildEnabled ();

            //         switch (__fpsState) 
            //{
            //case FieldDataEnabledState.ALWAYS_ENABLED:
            //	ScreenCore.instance.AddDebugScreen ();				
            //	break;
            //case FieldDataEnabledState.ENABLED_BUILD:
            //	if(Application.isEditor == false)
            //		ScreenCore.instance.AddDebugScreen ();				
            //	break;				
            //case FieldDataEnabledState.ENABLED_EDITOR:
            //	if(Application.isEditor)
            //		ScreenCore.instance.AddDebugScreen ();				
            //	break;
            //case FieldDataEnabledState.DISABLED:
            //	break;
            //}
        }

        /// <summary>
        /// Function to be called from the module core to send itself to this core.
        /// </summary>
        public void AddModuleCore(ModuleCore p_moduleCore)
        {
            _moduleCore = p_moduleCore;
        }

        /// <summary>
        /// Function to be called from the scene core to send itself to this core.
        /// </summary>
        public void AddSceneCore(SceneCore p_sceneCore)
        {
            _sceneCore = p_sceneCore;
        }

        /// <summary>
        /// Function to be called from the screen core to send itself to this core.
        /// </summary>
        public void AddScreenCore(ScreenCore p_screenCore)
        {
            _screenCore = p_screenCore;
        }
        #endregion

        #region Unity Default Monobehaviour Messages
        /// <summary>
        /// Default start of unity. Used only to initialize internal core information. Not running other conceptual starts of the framework.
        /// </summary>
        void Start()
        {
            CheckIfDebugFPSEnabled();

            _mainThread = System.Threading.Thread.CurrentThread;

            SetCurrentCultureInfo("en-US");

            if (Application.isEditor)
            {
                CheckIfMuteAudio();
            }
        }
        /// <summary>
        /// Default update of unity. Here our game is updated and initialized when required, controlling the main loop of the game.
        /// </summary>
        void Update()
        {
            if (_moduleCore != null)
                _moduleCore.ModulesUpdate();

            if (_sceneCore != null)
                _sceneCore.ScenesUpdate();

            if (_screenCore != null)
                _screenCore.ScreensUpdate();
        }
        /// <summary>
        /// Default fixed update of unity. Run every physics step of the game, being ideal for calculating physics motions.
        /// </summary>
        void FixedUpdate()
        {
            if (_moduleCore != null)
                _moduleCore.ModulesFixedUpdate();

            if (_sceneCore != null)
                _sceneCore.ScenesFixedUpdate();
        }
        /// <summary>
        /// Default updated called right before the end of frame. Here it is guaranteed to all other updates already have been run.
        /// Ideal for camera movements or other elements that require conclusion of other objets first.
        /// </summary>
        void LateUpdate()
        {
            if (_moduleCore != null)
                _moduleCore.ModulesLateUpdate();

            if (_sceneCore != null)
                _sceneCore.ScenesLateUpdate();
        }
        /// <summary>
        /// Used for direct screen drawing. Called multiples times per frame, being a very costfull loop, avoid developing logic inside this function.
        /// Use only for objective drawing.
        /// </summary>
        void OnGUI()
        {
            if (_moduleCore != null)
                _moduleCore.ModulesOnGUI();

            if (_sceneCore != null)
                _sceneCore.ScenesOnGUI();


            if (_screenCore != null)
                _screenCore.ScreensOnGUI();
        }
        /// <summary>
        /// Called when application is paused or sent to background, usefull for mobiles devices to check when user minimes the application.
        /// </summary>
        void OnApplicationPause(bool p_pauseStatus)
        {
            if (_sceneCore != null)
                _sceneCore.ScenesOnApplicationPaused(p_pauseStatus);
        }
        /// <summary>
        /// Called when application is returned and coming from background, usefull for mobiles devices to check when user maximizes the application.
        /// </summary>
        void OnApplicationFocus(bool p_focusStatus)
        {
            if (_sceneCore != null)
                _sceneCore.ScenesOnApplicationFocused(p_focusStatus);
        }
        /// <summary>
        /// Called when application is about to quit. Usefull or delete data if required.
        /// </summary>
        void OnApplicationQuit()
        {
            if (_sceneCore != null)
                _sceneCore.ScenesOnApplicationQuit();
        }
        #endregion

        #region Unity Main Thread
        public bool IsMainThread()
        {
            return _mainThread.Equals(System.Threading.Thread.CurrentThread);
        }

        private void SetCurrentCultureInfo(string p_cultureInfoName)
        {
            if (_mainThread.CurrentCulture.Name == p_cultureInfoName) return;

            System.Globalization.CultureInfo __cultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(p_cultureInfoName);

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = __cultureInfo;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = __cultureInfo;

            _mainThread.CurrentCulture = __cultureInfo;
            _mainThread.CurrentUICulture = __cultureInfo;
        }
        #endregion
    }
}
