#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System;
using System.Collections;
using VoxInternal;

namespace Vox
{
	/// <summary>
	/// Class used to create TimerNode to delay functions.
	/// </summary>
	/// <remarks>
	/// The timer class is used to call functions that creates Timers with the paramaters specified.
	/// Timers have a lot of uses, usually beeing related to making delayed events, such as the reload timer of a shooter game.
	/// 
	/// <code>
	///
	/// float _bullets = 10;
	/// Vox.TimerNode _reloadBulletsTimerNode = null;
	/// 
	/// public void ShootInput()
	/// {
	/// 	if(_bullets > 0)
	/// 	{
	/// 		_bullets -= 1;
	/// 
	/// 		FireBullet();
	/// 
	/// 		if(_bullets == 0)
	/// 		{
	/// 			// After 5 seconds reload player bullets.
	/// 
	/// 			_reloadBulletsTimerNode = Vox.Timer.WaitSeconds(5, delegate()
	/// 			{
	/// 				_bullets = 10;
	/// 			});
	/// 		}
	/// 	}
	/// }
	/// 
	/// </code>
	/// 
	/// </remarks>
	public class Timer 
	{
		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		#region Timed Function
		/// <summary>
		/// Function to call an action after the passed seconds value.
		/// </summary>
		/// <remarks>
		/// Function to call an action after the passed seconds value.
		/// Changing the Vox.UpdateTimeType can make the timer only be called after every update has hapenned, for example, by changing it to Vox.UpdateTimeType.LATE_UPDATE.
		/// <code>
		///
		/// Vox.TimerNode _debugTimerNode = null;
		/// 
		/// _debugTimerNode = Vox.Timer.WaitSeconds(1, false, 0, true, UpdateTimeType.LATE_UPDATE, delegate()
		/// {	
		/// 	Debug.log(_player.transform.position); // Will debug player position after the player update which has the movement happended.
		/// });
		/// 
		/// </code>
		/// 
		/// </remarks>
		private static TimerNode TimedFunction(float p_secondsToWait, bool p_isLoop, int p_framesToWait, int p_numberOfLoopIterations, bool p_useScaleTime, UpdateTimeType p_updateMode, Action p_callback)
		{
			TimerNode __node = null;

			Action __actionUpdate = delegate
			{
				if(__node.currentState == NodeState.PLAYING)
				{
					if(__node.frameCounter >= __node.framesToWait)
					{
						if(__node.useTimeScale)
							__node.currentTimeStep += Time.deltaTime;
						else
							__node.currentTimeStep += Time.unscaledDeltaTime;

						__node.currentTimeStep = Mathf.Min(__node.currentTimeStep, p_secondsToWait);

						if (__node.currentTimeStep >= __node.timeToWait)
						{
							if(__node.usingLoop)
							{
								__node.loopIteration++;

								if(p_callback != null)
									p_callback();

								if(__node.loopIteration < p_numberOfLoopIterations || p_numberOfLoopIterations == 0)
								{
									__node.currentTimeStep = 0;
								}
								else
								{
									__node.SetNodeToBeCleaned();
								}
							}
							else
							{
								if(p_callback != null)
									p_callback ();								

								__node.SetNodeToBeCleaned();
							}
						}
					}
					else
					{
						__node.frameCounter++;
					}
				}
			};

			Action __actionAnticipate = delegate 
			{
				if(p_callback != null)
					p_callback ();
				
				__node.SetNodeToBeCleaned();
			};

			__node = new TimerNode(p_secondsToWait, p_framesToWait, p_useScaleTime, p_updateMode, p_isLoop, TimerModule.instance.GetNextNodeID(), p_callback, __actionUpdate, __actionAnticipate);

			TimerModule.instance.AddNode(__node);

			return __node;
		}
		#endregion


		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region Wait Frames
		/// <summary>
		/// <STRONG>(2 overloads)</STRONG> Function to call an action after the passed frame value.
		/// </summary>
		/// <remarks>
		/// Function to call an action after the passed frame value.
		///  
		/// Useful for handling small timer issues, like corroutines that may start only after a while.
		/// Used to avoid performance spikes after big calculations, such as the loading of scenes and others. 
		/// 
		/// <STRONG><I>TimerNode WaitFrames(int p_frames, Action p_callbackMethod)</I></STRONG>
		/// 
		/// <code>
		/// public void SceneAwake()
		/// { 
		/// 	//Start a fade only after the two first frames since scene awaken.
		/// 	Vox.Timer.WaitFrames(2, delegate()
		/// 	{
		/// 		Vox.ScreenManager.FadeScreen(FadeScreenType.FADE_IN_BLACK, 1, EaseType.Linear, null);
		/// 	});
		/// }
		/// </code>
		/// 
		/// <STRONG><I>TimerNode WaitFrames(int p_frames, bool p_isLoop, Action p_callbackMethod)</I></STRONG>
		/// 
		/// Has a loop parameter for special cases such as perform calculations every x frames.
		/// 
		/// <code>
		///
		/// //Start a fade only after the two first frames since scene awaken.
		/// Vox.TimerNode __takeScreenShotNode = null;
		/// 
		/// __takeScreenShotNode = Vox.Timer.WaitFrames(360, true, delegate()
		/// {
		/// 	Application.CaptureScreenshot("FrameScreenshot" + (360 *__takeScreenShotNode.loopIteration));
		/// });
		/// 
		/// </code> 
		/// 
		/// </remarks>
		public static TimerNode WaitFrames(int p_frames, Action p_callbackMethod)
		{
			return TimedFunction(0, false, p_frames, 0, false, UpdateTimeType.UPDATE, p_callbackMethod);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TimerNode WaitFrames(int p_frames, bool p_isLoop, Action p_callbackMethod)
		{
			return TimedFunction(0, p_isLoop, p_frames, 0, false, UpdateTimeType.UPDATE, p_callbackMethod);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region Wait seconds
		/// <summary>
		/// <STRONG>(5 overloads)</STRONG> Function to call an action after the passed seconds value.
		/// </summary>
		/// <remarks>
		/// Function to call an action after the passed seconds value.
		///  
		/// Useful for any logic related to time, usually delays or time checks. 
		/// Such as a reloading delay, start game delay, match time...
		/// 
		/// <STRONG><I>TimerNode WaitSeconds(float p_secondsToWait, Action p_callbackMethod)</I></STRONG>
		/// 
		/// <code>
		///
		/// float _bullets = 10;
		/// Vox.TimerNode _reloadBulletsTimerNode = null;
		/// 
		/// public void ShootInput()
		/// {
		/// 	if(_bullets > 0)
		/// 	{
		/// 		_bullets -= 1;
		/// 
		/// 		FireBullet();
		/// 
		/// 		if(_bullets == 0)
		/// 		{
		/// 			// After 5 seconds reload player bullets.
		/// 
		/// 			_reloadBulletsTimerNode = Vox.Timer.WaitSeconds(5, delegate()
		/// 			{
		/// 				_bullets = 10;
		/// 			});
		/// 		}
		/// 	}
		/// }
		/// </code>
		/// 
		/// <STRONG><I>TimerNode WaitSeconds(float p_secondsToWait, bool p_isLoop, Action p_callbackMethod)</I></STRONG>
		/// 
		/// The loop parameters help in actions that happens once every x times. Important to link it to a node instance so you can cancel easily later.
		/// 
		/// <code>
		///
		/// Vox.TimerNode __annoyingNode = null;
		/// 
		/// __annoyingNode = Vox.Timer.WaitSeconds(5, true, delegate()
		/// {
		/// 	Debug.Log("Oi, eu sou Goku!");
		/// });
		/// 
		/// </code>
		/// 
		/// <STRONG><I>TimerNode WaitSeconds(float p_secondsToWait, bool p_isLoop, int p_numberOfLoopIterations, Action p_callbackMethod)</I></STRONG>
		/// 
		/// You can set a limited times for the loop to iterate.
		/// 
		/// <code>
		///
		/// Vox.TimerNode _countDownNode = null;
		/// int _numberOfTimesToIterate = 5;
		/// UnityEngine.UI.Text _timeText;
		/// 
		/// _countDownNode = Vox.Timer.WaitSeconds(1, true, _numberOfTimesToIterate, delegate()
		/// {	
		/// 	int __secondsToFinish = _numberOfTimesToIterate - _countDownNode.loopIteration;
		/// 
		/// 	_timeText.text = __secondsToFinish.ToString(); //Will write: 5/4/3/2/1 after each 1 second.
		/// 
		/// 	if(__secondsToFinish == 0)
		/// 	{
		/// 		Debug.log("FinishedCountdown");
		/// 	}
		/// 
		/// });
		/// 
		/// </code> 
		/// 
		/// <STRONG><I>TimerNode WaitSeconds(float p_secondsToWait, bool p_isLoop, int p_numberOfLoopIterations, bool p_useScaleTime, Action p_callbackMethod)</I></STRONG>
		/// 
		/// If not using TimeScale, will happen even if TimeScale is 0 (Normally when game is paused); 
		///
		/// <code>
		///
		/// Vox.TimerNode _countDownNode = null;
		/// int _numberOfTimesToIterate = 5;
		/// UnityEngine.UI.Text _timeText;
		/// 
		/// Vox.Time.timeScale = 0;
		/// 
		/// _countDownNode = Vox.Timer.WaitSeconds(1, true, _numberOfTimesToIterate, false, delegate()
		/// {	
		/// 	int __secondsToFinish = _numberOfTimesToIterate - _countDownNode.loopIteration;
		/// 
		/// 	_timeText.text = __secondsToFinish.ToString(); //Will write: 5/4/3/2/1 after each 1 second.
		/// 
		/// 	if(__secondsToFinish == 0)
		/// 	{
		/// 		Debug.log("FinishedCountdown");
		/// 	}
		/// });
		/// 
		/// </code>
		///  
		/// <STRONG><I>TimerNode WaitSeconds(float p_secondsToWait, bool p_isLoop, int p_numberOfLoopIterations, bool p_useScaleTime, bool p_useLateUpdate, Action p_callbackMethod)</I></STRONG>
		/// 
		/// You can set that the function to finish at the end of the frame using useLateUpdate when the time in seconds has passed.
		/// 
		/// </remarks>
		public static TimerNode WaitSeconds(float p_secondsToWait, Action p_callbackMethod)
		{
			return TimedFunction(p_secondsToWait, false, 0, 0, true, UpdateTimeType.UPDATE, p_callbackMethod);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TimerNode WaitSeconds(float p_secondsToWait, bool p_isLoop, Action p_callbackMethod)
		{
			return TimedFunction(p_secondsToWait, p_isLoop, 0, 0, true, UpdateTimeType.UPDATE, p_callbackMethod);
		}

		public static TimerNode WaitSeconds(float p_secondsToWait, bool p_isLoop, int p_numberOfLoopIterations, Action p_callbackMethod)
		{
			return TimedFunction(p_secondsToWait, p_isLoop, 0, p_numberOfLoopIterations, true,  UpdateTimeType.UPDATE, p_callbackMethod);
		}

		public static TimerNode WaitSeconds(float p_secondsToWait, bool p_isLoop, int p_numberOfLoopIterations, bool p_useScaleTime, Action p_callbackMethod)
		{
			return TimedFunction(p_secondsToWait, p_isLoop, 0, p_numberOfLoopIterations, p_useScaleTime, UpdateTimeType.UPDATE, p_callbackMethod);
		}
			
		public static TimerNode WaitSeconds(float p_secondsToWait, bool p_isLoop, int p_numberOfLoopIterations, bool p_useScaleTime, bool p_useLateUpdate, Action p_callbackMethod)
		{
			if(p_useLateUpdate)
				return TimedFunction(p_secondsToWait, p_isLoop, 0, p_numberOfLoopIterations, p_useScaleTime, UpdateTimeType.LATE_UPDATE, p_callbackMethod);
			else
				return TimedFunction(p_secondsToWait, p_isLoop, 0, p_numberOfLoopIterations, p_useScaleTime, UpdateTimeType.UPDATE, p_callbackMethod);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region Clean Nodes
		/// <summary>
		/// Clear all timer nodes
		/// </summary>
		/// <remarks>
		/// ClearAllNodes should be called to clean all timer nodes initialized via this Timer class. 
		/// Useful to make sure nothing is left behind when not intended.
		/// 
		/// <code>
		/// 
		/// _bullets = 10;
		/// 
		/// void StartReloadBulletsTimer()
		/// {
		/// 	Vox.TimerNode __reloadBullet = Vox.Timer.WaitSeconds(5f, delegate()
		/// 	{
		/// 		_bullets = 10;
		/// 	});
		/// }
		///
		/// void override OnUnloadedScene()
		/// {
		/// 	Vox.Tween.ClearAllNodes();
		/// }
		/// 
		/// </code>
		///  
		/// </remarks>  
		public static void ClearAllNodes()
		{
			TimerModule.instance.ClearAllNodes ();
		}

		/// <summary>
		/// Clear all timer nodes with the passed tag
		/// </summary>
		/// <remarks>
		/// ClearNodesWithTag clean all nodes which had the passed tag attached to them. For more information check Vox.TimerNode.tag.
		/// 
		/// <code>
		/// // Script inside a state of a statemachine
		/// 
		/// float _bullets = 0;
		/// 
		/// void StartReloadBulletsTimer( )
		/// {
		/// 	Vox.TimerNode __reloadBullet = Vox.Timer.WaitSeconds(5f, delegate()
		/// 	{
		/// 		_bullets = 10;
		/// 	});
		/// 
		/// 	__reloadBullet.tag = "GamePlayState";
		/// }
		///
		/// void override StateOnUnloadedScene()
		/// {
		/// 	Vox.Tween.ClearNodesWithTag("GamePlayState");
		/// }
		/// </code>
		///  
		/// </remarks> 
		public static void ClearNodesWithTag(string p_tag)
		{
			TimerModule.instance.ClearNodesWithTag (p_tag);
		}

		/// <summary>
		/// Clear all timer nodes excpet for the ones with the passed tag
		/// </summary>
		/// <remarks>
		/// ClearNodesExceptForTag clean all nodes except for the ones with the passed tag attached to them. For more information check Vox.TimerNode.tag.
		/// 
		/// <code>
		///
		/// void override OnUnloadedScene()
		/// {
		/// 	Vox.Timer.ClearNodesExceptForTag("GlobalNodes");
		/// }
		/// 
		/// </code>
		///   
		/// </remarks> 
		public static void ClearNodesExceptForTag(string p_tag)
		{
			TimerModule.instance.ClearNodesExceptForTag (p_tag);
		}

		#endregion
	}
}