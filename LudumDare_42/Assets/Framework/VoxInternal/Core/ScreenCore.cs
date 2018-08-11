using UnityEngine;
using System.Collections;
using System;
using Vox;

namespace VoxInternal
{
	/// <summary>
	/// The class that updates and manages specialized classes related to the drawing on the screen. Such as fade textures and debug consoles. 
	/// </summary>
	public class ScreenCore
	{
		#region FPSDebug Properties
		private FpsScreen _debugScreen;
		#endregion

		#region Fade Properties
		private Texture2D _fadeTexture;
		private FadeScreenType _fadeType = FadeScreenType.NONE;
		private float _fadeAlpha = 0;
        private float _startAlpha = 0f;
        private float _finalAlpha = 1f;
        private float _timeStep = 0;
        private  float _duration = 0;
        private  EaseType _curve = EaseType.LINEAR;
        private  bool _useDeltaTime = true;
        private  bool _isFading = false;

        private  Action _callback = null;
		#endregion

		#region Instance Initialization
		/// <summary>
		/// Create automatically the unique instance of this class.
		/// </summary>
		static private ScreenCore _instance;
		static public ScreenCore instance
		{
			get 
			{ 
				if (_instance == null)
				{
					_instance = InstanceInitialize ();
				}

				return _instance;
			}
		}
		/// <summary>
		/// Handles the logic required in the inicialization of the instance.
		/// </summary>		
		static private ScreenCore InstanceInitialize()
		{
			ScreenCore __instance = new ScreenCore ();

			GameCore.instance.AddScreenCore (__instance);

			return __instance;
		}
		#endregion

		#region Add
		/// <summary>
		/// Function to be called from the Game Core to add the debug screen if setted to be activated on the Framework Window.
		/// </summary>
		public void AddDebugScreen()
		{
			if (_debugScreen == null)
				_debugScreen = new FpsScreen ();
		}

		#endregion

		#region
		/// <summary>
		/// Function that updates the drawings on the screen. Updating fade interpolations and debug information.
		/// </summary>
		public void ScreensUpdate()
		{
            UpdateFade();

			//if (_debugScreen != null)
			//{
			//	_debugScreen.UpdateFPS();
			//}
		}
		/// <summary>
		/// Function that actually perform the drawings on the screen. Being calle dmultiple times per frame.
		/// </summary>
		public void ScreensOnGUI()
		{
			//Fade update
			if (_fadeType != FadeScreenType.NONE)
			{
				Color __color = GUI.color;
				__color.a = _fadeAlpha;
				GUI.color = __color;
				GUI.DrawTexture(new Rect(0,0, UnityEngine.Screen.width, UnityEngine.Screen.height), _fadeTexture, ScaleMode.StretchToFill, false);
			}

			if (_debugScreen != null)
			{
				_debugScreen.OnFPSGUI();
			}
		}

		#endregion

		#region Fade Control
		/// <summary>
		/// Function that starts a custom tween fade to be drawn on the screen. Remember to clear it on the callback or call another fade to do it.
		/// </summary>
		public void StartFadeScreen(FadeScreenType p_fadeType, float p_duration, EaseType p_curve, bool p_useDeltaTime, Action p_callback)
		{
			if (p_duration == 0)
			{
				_fadeType = p_fadeType;

				switch (_fadeType)
				{
					case FadeScreenType.FADE_IN_BLACK:
					_fadeAlpha = 0f;
					_fadeTexture = Texture2D.blackTexture;
					break;
					case FadeScreenType.FADE_IN_WHITE:
					_fadeAlpha = 0f;
					_fadeTexture = Texture2D.whiteTexture;
					break;
					case FadeScreenType.FADE_OUT_BLACK:
					_fadeAlpha = 1f;
					_fadeTexture = Texture2D.blackTexture;
					break;
					case FadeScreenType.FADE_OUT_WHITE:
					_fadeAlpha = 1f;
					_fadeTexture = Texture2D.whiteTexture;
					break;
					default:
					break;
				}
			}
			else
			{
				if (_isFading)
				{
					_callback?.Invoke();
				}

				_timeStep = 0;
				_fadeType = p_fadeType;
				_duration = p_duration;
				_curve = p_curve;
				_useDeltaTime = p_useDeltaTime;
				_callback = p_callback;
				_isFading = true;

				switch (_fadeType)
				{
					case FadeScreenType.FADE_IN_BLACK:
                    _startAlpha = 1f;
					_finalAlpha = 0f;
					_fadeTexture = Texture2D.blackTexture;
					break;
					case FadeScreenType.FADE_IN_WHITE:
					_startAlpha = 1f;
					_finalAlpha = 0f;
					_fadeTexture = Texture2D.whiteTexture;
					break;
					case FadeScreenType.FADE_OUT_BLACK:
					_startAlpha = 0f;
					_finalAlpha = 1f;
					_fadeTexture = Texture2D.blackTexture;
					break;
					case FadeScreenType.FADE_OUT_WHITE:
					_startAlpha = 0f;
					_finalAlpha = 1f;
					_fadeTexture = Texture2D.whiteTexture;
					break;
					default:
					break;
				}

                _fadeAlpha = _startAlpha;
            }
        }

        private void UpdateFade()
        {
            if(_isFading)
            {
                if (_timeStep < _duration)
                {
                    if(_useDeltaTime)
                        _timeStep += Time.deltaTime;
                    else
                        _timeStep += Time.unscaledDeltaTime;

                    _fadeAlpha = EaseFunctionMaths.GetTransaction( _startAlpha, _finalAlpha, _timeStep/_duration, _curve);
                }
                else
                {
                    _timeStep = 0;
                    _duration = 0;
                    _isFading = false;

                    _callback?.Invoke();
                }
            }
        }


		public void ClearCallback()
		{
			_callback = null;
		}
		/// <summary>
		/// Function that clears any fade drawn to the screen.
		/// </summary>
		public void ClearFade()
		{
			_fadeAlpha = 0;
			_fadeType = FadeScreenType.NONE;
		}
		#endregion
	}
}