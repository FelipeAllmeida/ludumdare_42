#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VoxInternal;

namespace Vox
{
	/// <summary>
	/// Instance of a generated timer that callbacks a function.
	/// </summary>
	/// <remarks>
	/// A timernode stores a delayed function, allowing a variety of parameters to control when  to call this function.
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
	/// 			// After 10 seconds reload player bullets.
	/// 
	/// 			_reloadBulletsTimerNode = Vox.Timer.WaitSeconds(10, delegate()
	/// 			{
	/// 				_bullets = 10;
	/// 			});
	/// 			
	/// 			_reloadBulletsTimerNode.tag = "GamePlayState";
	/// 		}
	/// 	}
	/// }
	///
	/// void CollectedReloadSpeedUp()
	/// {
	/// 	if(_reloadBulletsTimerNode != null)
	/// 		_reloadBulletsTimerNode.AnticipateSeconds(5);
	/// }
	/// 
	/// </code>
	///  
	/// </remarks> 
	[Serializable]
	public class TimerNode 
	{
		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		private Action _delegatedCallback;
		private Action _onUpdate;
		private Action _onAnticipate;

		private int _id;
		private int _framesToWait = 0;
		private float _timeToWait = 0;
		private bool _usingLoop;
		private bool _useTimeScale;
		private NodeState _currentState;
		private UpdateTimeType _updateMode;
		private bool _shouldNodeBeCleaned = false;

		#region Constructor
		/// <summary>
		/// Constructor of TimerNode. Used internally by the Timer class to create the timer, do not use it.
		/// </summary>
		/// <remarks>
		/// Constructor of TimerNode. Used internally by the Timer class to create the timer, do not use it.
		/// </remarks>
		public TimerNode(float p_time, int p_framesToWait, bool p_useTimeScale, UpdateTimeType p_updateMode, bool p_useLoop, int p_id, Action p_callbackUser, Action p_actionUpdate, Action p_actionAnticipate)
		{
			//Initialize Data
			_id = p_id;
			_timeToWait = p_time;
			_framesToWait = p_framesToWait;
			_updateMode = p_updateMode;
			_usingLoop = p_useLoop;
			currentTimeStep = 0f;
			_useTimeScale = p_useTimeScale;

			_delegatedCallback = p_callbackUser;
			_onUpdate = p_actionUpdate;
			_onAnticipate = p_actionAnticipate;

			//StartPlaying
			ChangeState(NodeState.PLAYING);
		}
		#endregion

		/// <summary>
		/// Change state of node
		/// </summary>
		private void ChangeState(NodeState p_state)
		{
			_currentState = p_state;
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		/// <summary>
		/// Callback function passed via the TweenFunction. Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// Callback function passed via the TweenFunction. Used for debug and get only.
		/// </remarks>
		public Action delegatedCallback { get { return _delegatedCallback; } }

		#region Public Data
		/// <summary>
		/// Tag which can be used later to identificate and relate this tweens to others. Allowing them to be cleaned separedly if required.
		/// </summary>
		/// <remarks>
		/// Tags are useful to be set when you are creating tweens that should stay even if their a scene is unloaded, or if you are using several tweens related to a state of a state machine and want to clear them easily when changing a state.
		/// You can use the tags manually in your own or use the functions Tween.ClearNodesExceptForTag() or Tween.ClearNodesWithTag().
		/// 
		/// Example of a tag being set is show below:
		/// 
		/// <code>
		/// // Script inside a state of a statemachine
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
		/// 			
		/// 			_reloadBulletsTimerNode.tag = "GamePlayState";
		/// 		}
		/// 	}
		/// }
		///
		/// void override StateOnUnloadedScene()
		/// {
		/// 	Vox.Tween.ClearNodesWithTag("GamePlayState");
		/// }
		/// </code>
		/// 
		/// </remarks>
		public string tag = "";
		/// <summary>
		/// GameObject that can be associated to a tween, useful for debugging and checking which gameObjets relates to each tween.
		/// </summary>
		/// <remarks>
		/// GameObject that can be associated to a tween, useful for debugging and checking which gameObjets relates to each tween.
		/// 
		/// Example of a associatedObject being set is show below:
		/// 
		/// <code>
		/// 
		/// void DeleteSphereTimer(GameObejct p_sphere)
		/// {
		/// 	Vox.TimerNode __deleteSphereTimer = Vox.Timer.WaitSeconds(5, delegate()
		/// 	{
		/// 		Destroy(p_sphere)
		/// 	});
		/// 
		/// 	__deleteSphereTimer.associatedObject = p_sphere;
		/// }
		//
		/// </code>
		/// 
		/// </remarks>
		public GameObject associatedObject;

		/// <summary>
		/// CurrentTimeStep is the time current passed since the creation and the running time of the node.
		/// </summary>
		/// <remarks>
		/// CurrentTimeStep is the time current passed since the creation and the running time of the node. (Pause timer node pauses the increase of this). 
		/// Useful only for debug operations, do not change it directly or the timer will not function properly.
		/// </remarks>
		public float currentTimeStep;

		/// <summary>
		/// Stores the frames passed if the timer use frames delay. 
		/// </summary>
		/// <remarks>
		/// Similar to Vox.TimerNode.CurrentTimeStep, it is the current frames passed since the creation and the running time of the node. (Pause timer node pauses the increase of this). 
		/// Useful only for debug operations, do not change it directly or the timer will not function properly.
		/// </remarks>
		public int frameCounter;

		/// <summary>
		/// Iteration of the TimerNode if it is a loop.
		/// </summary>
		/// <remarks>
		/// Iteration of the TimerNode if it is a loop. Useful for debug and a few logics.
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
		/// </remarks>
		public int loopIteration;
		#endregion

		#region Private Get-Only Data
		/// <summary>
		/// Unique id among others TimerNodes. Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// Unique id among others TimerNodes. Used for debug and get only.
		/// </remarks>
		public int id { get { return _id; } }

		/// <summary>
		/// Current state of timer node. Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// Current state of timer node. Used for debug and get only.
		/// </remarks>
		public NodeState currentState{get{return _currentState;}}

		/// <summary>
		/// UpdateTimeType to be used in tween passed via Vox.Timer.  Defines the type of update of the game or if it has one. Get only.
		/// </summary>
		/// <remarks>
		/// UpdateTimeType to be used in tween passed via Vox.Timer. Defines the type of update of the game or if it has one. Get only.
		/// </remarks>
		public UpdateTimeType updateMode{get{return _updateMode;}}

		/// <summary>
		/// Time in seconds to wait to call the passed action.
		/// </summary>
		/// <remarks>
		/// Time in seconds to wait to call the passed action. Defined via Vox.Timer call.
		/// </remarks>
		public float timeToWait{get{return _timeToWait;}}

		/// <summary>
		/// Time in frames to wait to call the passed action.
		/// </summary>
		/// <remarks>
		/// Time in frames to wait to call the passed action. Defined via Vox.Timer call.
		/// </remarks>
		public int framesToWait{get{return _framesToWait;}}

		/// <summary>
		/// usingLoop defines if timer node is loop, defined via Vox.Timer when creating the node.
		/// </summary>
		/// <remarks>
		/// usingLoop defines if timer node is loop, defined via Vox.Timer when creating the node.
		/// </remarks>
		public bool usingLoop{get{return _usingLoop;}}

		/// <summary>
		/// Defines if the timer will use Unity TimeScale or ignore it. Get only.
		/// </summary>
		/// <remarks>
		/// Defines if the timer will use Unity TimeScale or ignore it. Get only.
		/// </remarks>
		public bool useTimeScale{get{return _useTimeScale;}}
		#endregion

		#region UpdateNode
		/// <summary>
		/// Used internally by the TimerModule class to update the timer, do not use it.
		/// </summary>
		/// <remarks>
		/// Used internally by the TimerModule class to update the timer, do not use it.
		/// </remarks>
		public void UpdateNode()
		{
			if (_onUpdate != null) 
				_onUpdate();
		}
		#endregion

		#region Change Time Functions
		/// <summary>
		/// Anticipate time to finish node.
		/// </summary>
		/// <remarks>
		/// Anticipateseconds increases the currentTimeStep of the node to reach the timeToWait faster and call the function stored.
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
		/// 			// After 10 seconds reload player bullets.
		/// 
		/// 			_reloadBulletsTimerNode = Vox.Timer.WaitSeconds(10, delegate()
		/// 			{
		/// 				_bullets = 10;
		/// 			});
		/// 			
		/// 			_reloadBulletsTimerNode.tag = "GamePlayState";
		/// 		}
		/// 	}
		/// }
		///
		/// void CollectedReloadSpeedUp()
		/// {
		/// 	if(_reloadBulletsTimerNode != null)
		/// 		_reloadBulletsTimerNode.AnticipateSeconds(5);
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks>
		public void AnticipateSeconds(float p_seconds)
		{
			Debug.Log (currentTimeStep);

			currentTimeStep += Mathf.Abs(p_seconds);
			currentTimeStep = Mathf.Clamp (currentTimeStep, currentTimeStep, _timeToWait);

		}

		/// <summary>
		/// Delay time to finish node.
		/// </summary>
		/// <remarks>
		/// DelaySeconds decreases the currentTimeStep of the node, making it further from the end of the timer.
		/// You can add freely more seconds to wait.
		/// 
		/// <code>
		/// 
		/// Vox.TimerNode __timeOutNode = null;
		/// 
		/// public void StartGameTimer()
		/// {
		/// 	__timeOutNode = Vox.Timer.WaitSeconds(60, delegate()
		/// 	{	
		/// 		Debug.Log("FinishGame!");
		/// 	});
		/// }
		///
		/// void CollectedExtraTime()
		/// {
		/// 	if(__timeOutNode != null)
		/// 		_reloadBulletsTimerNode.DelaySeconds(10);
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks> 
		public void DelaySeconds(float p_seconds)
		{
			currentTimeStep -= Mathf.Abs(p_seconds);
		}
		#endregion

		#region Node Control Functions
		/// <summary>
		/// Resumes the timer if it was paused.
		/// </summary>
		/// <remarks>
		/// Resumes the timer if it was paused.
		///
		/// <code>
		/// 
		/// Vox.TimerNode __timeOutNode = null;
		/// 
		/// public void StartGameTimer()
		/// {
		/// 	__timeOutNode = Vox.Timer.WaitSeconds(60, delegate()
		/// 	{	
		/// 		Debug.Log("FinishGame!");
		/// 	});
		/// }
		///
		/// void CallCutscene()
		/// {
		/// 	if(__timeOutNode != null)
		/// 		__timeOutNode.Pause();
		/// 
		/// 	PlayCustscene(delegate() // PlayCutscene would have a finish callback
		/// 	{
		/// 		///__timeOutNode.Resume();
		/// 	});
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks>
		public void Resume()
		{
			ChangeState(NodeState.PLAYING);
		}

		/// <summary>
		/// Pause the timer if it was running.
		/// </summary>
		/// <remarks>
		/// Pause the timer if it was running.
		///
		/// <code>
		/// 
		/// Vox.TimerNode __timeOutNode = null;
		/// 
		/// public void StartGameTimer()
		/// {
		/// 	__timeOutNode = Vox.Timer.WaitSeconds(60, delegate()
		/// 	{	
		/// 		Debug.Log("FinishGame!");
		/// 	});
		/// }
		///
		/// void CallCutscene()
		/// {
		/// 	if(__timeOutNode != null)
		/// 		__timeOutNode.Pause();
		/// 
		/// 	PlayCustscene(delegate() // PlayCutscene would have a finish callback
		/// 	{
		/// 		///__timeOutNode.Resume();
		/// 	});
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks> 
		public void Pause()
		{
			ChangeState(NodeState.PAUSED);
		}
			
		/// <summary>
		/// Cancel this timer without calling its delegated callback.
		/// </summary>
		/// <remarks>
		/// Cancel this timer without calling its delegated callback.
		///
		/// <code>
		/// 
		/// Vox.TimerNode __bombTimer = null;
		/// 
		/// public void InitiateBombTimer()
		/// {
		/// 	__bombTimer = Vox.Timer.WaitSeconds(60, delegate()
		/// 	{	
		/// 		Debug.Log("BUUUM!");
		/// 	});
		/// }
		///
		/// void CutRightBomWire()
		/// {
		/// 	if(__bombTimer != null)
		/// 		__bombTimer.Cancel();
		/// 
		/// 	//Bomb will not explode.
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks> 
		public void Cancel()
		{
			_shouldNodeBeCleaned = true;
		}

		/// <summary>
		/// Force timer to go to last value and call its delegated callback.
		/// </summary>
		/// <remarks>
		/// Force timer to go to last value and call its delegated callback.
		///
		/// <code>
		/// 
		/// Vox.TimerNode __bombTimer = null;
		/// 
		/// public void InitiateBombTimer()
		/// {
		/// 	__bombTimer = Vox.Timer.WaitSeconds(60, delegate()
		/// 	{	
		/// 		Debug.Log("BUUUM!");
		/// 	});
		/// }
		///
		/// void CutWrongWire()
		/// {
		/// 	if(__bombTimer != null)
		/// 		__bombTimer.Anticipate();
		/// 
		/// 	//Bomb will explode!
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks>  
		public void Anticipate()
		{
			if(_onAnticipate != null) _onAnticipate();
		}

		public void SetNodeToBeCleaned()
		{
			_shouldNodeBeCleaned = true;
		}

		public bool ShouldNodeBeCleaned()
		{
			return _shouldNodeBeCleaned;
		}
		#endregion
		
	}
}