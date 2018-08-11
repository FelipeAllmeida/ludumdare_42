#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System.Collections;
using System;
using VoxInternal;

namespace Vox
{
	#region Enumerations
	/// <summary>
	/// Enum with fade types availables.
	/// </summary>
	/// <remarks>
	/// Fade type used by FadeScreen function to set color of fade and define if it is a fade in or fade out.
	/// </remarks>
	public enum FadeScreenType
	{
		NONE,
		FADE_IN_BLACK,
		FADE_IN_WHITE,
		FADE_OUT_BLACK,
		FADE_OUT_WHITE
	};
	#endregion

	/// <summary>
	/// Class to manage screen related functions.
	/// </summary>
	/// <remarks>
	/// Class to manage screen related functions. Currently only fade functions are available from here.
	/// 
	/// <code>
	/// 
	/// function FadeSceneThenClearFade()
	/// {
	/// 	Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_OUT_BLACK, 1, Vox.EaseType.Linear, delegate()
	/// 	{
	/// 		Vox.ScreenManager.ClearFadeScreen(); // Will clear the black that is in the screen due to the fade out.
	/// 	});
	/// }
	/// 
	/// </code> 
	/// 
	/// </remarks> 
	public static class ScreenManager
	{
		public static void ClearCallback()
		{
			ScreenCore.instance.ClearCallback ();
		}

		#region Fade Methods
		/// <summary>
		/// If screen has a fade being drawed with OnGUI clear it.
		/// </summary>
		/// <remarks>
		/// If screen has a fade being drawed with OnGUI clear it.
		/// 
		/// <code>
		/// 
		/// function FadeSceneThenClearFade()
		/// {
		/// 	Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_OUT_BLACK, 1, Vox.EaseType.Linear, delegate()
		/// 	{
		/// 		Vox.ScreenManager.ClearFadeScreen(); // Will clear the black that is in the screen due to the fade out.
		/// 	});
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks> 
		public static void ClearFadeScreen()
		{
			ScreenCore.instance.ClearFade ();
		}

		/// <summary>
		/// Fade screen with passed fade type and parameters.
		/// </summary>
		/// <remarks>
		/// <STRONG>(2 overloads)</STRONG> Fade screen with passed fade type and parameters. Has a useful finish callback.
		/// 
		/// <STRONG><em>void FadeScreen(FadeScreenType p_fadeType, float p_duration, EaseType p_curve, Action p_callback)</em></STRONG>
		/// 
		/// <code>
		/// 
		/// function FadeSceneThenClearFade()
		/// {
		/// 	Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_OUT_BLACK, 1, Vox.EaseType.Linear, delegate()
		/// 	{
		/// 		Vox.ScreenManager.ClearFadeScreen(); // Will clear the black that is in the screen due to the fade out.
		/// 	});
		/// }
		/// 
		/// 
		/// </code>
		/// 
		/// <STRONG><em>void FadeScreen(FadeScreenType p_fadeType, float p_duration, EaseType p_curve, bool p_useDeltaTime, Action p_callback)</em></STRONG>
		/// 
		/// <code>
		/// 
		/// function FadeSceneThenClearFade()
		/// {
		/// 	Time.timeScale = 0f;
		/// 
		/// 	Vox.ScreenManager.FadeScreen(Vox.FadeScreenType.FADE_OUT_BLACK, 1, Vox.EaseType.Linear, false, delegate()
		/// 	{
		/// 		Vox.ScreenManager.ClearFadeScreen(); // Will clear the black that is in the screen due to the fade out.
		/// 	});
		/// }
		/// 
		/// 
		/// </code>
		/// </remarks>  
		public static void FadeScreen(FadeScreenType p_fadeType, float p_duration, EaseType p_curve, Action p_callback)
		{
			ScreenCore.instance.StartFadeScreen(p_fadeType, p_duration, p_curve, true, p_callback);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static void FadeScreen(FadeScreenType p_fadeType, float p_duration, EaseType p_curve, bool p_useDeltaTime, Action p_callback)
		{
			ScreenCore.instance.StartFadeScreen(p_fadeType, p_duration, p_curve, p_useDeltaTime, p_callback);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion
	}
}
