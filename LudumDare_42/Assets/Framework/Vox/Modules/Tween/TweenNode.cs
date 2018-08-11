#define SKIP_CODE_IN_DOCUMENTATION


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VoxInternal;
using System.ComponentModel;

namespace Vox
{
	/// <summary>
	/// Type of loop availabes to be used by tween node.
	/// </summary>
	/// <remarks>
	/// TweenLoopMode defines if the tween has no loop, NONE which is usually the default type of tween calls.
	/// If it is a LOOP which repeats itself when finished, or if it is a PING_PONG, a loop that when reaches the end value, interpolates this to go back to the start one.
	/// </remarks>
	public enum TweenLoopType
	{
		NONE,
		LOOP,
		PING_PONG
	}

	/// <summary>
	/// Instance of a generated tween to lerp between values.
	/// </summary>
	/// <remarks>
	/// A tween node is an instance of a tween, which interpolates two values depending on its parameters. Useful for animation, movements and many others.
	/// 
	/// Code example:
	/// 
	/// <code>
	/// 
	/// Vox.TweenNode __showSpriteNode = Vox.Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_alpha)
	/// {
	/// 	Color __newColor = p_renderer.color;
	/// 	__newColor.a = p_alpha;
	/// 	p_renderer.color = __newColor;
	/// });
	/// 
	/// __showSpriteNode.onFinish += delegate()
	/// {
	/// 	Vox.TweenNode __changeSpriteColor = Vox.Tween.ColorTo(Color.white, Color.red, 1f, EaseType.Linear, delegate(Color p_color) 
	/// 	{
	/// 		p_renderer.color = p_color;	
	/// 	}
	/// };
	/// 
	/// </code>
	///  
	/// </remarks> 
	[Serializable]
	public class TweenNode 
	{
		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		private Action<float> _delegatedCallback;
		private Action _onUpdate;
		private Action _onAnticipate;

		private int _id;
		private NodeState _currentState;
		private float _startValue = 0;
		private float _endValue = 0;
		private float _duration = 0;
		private bool _useTimeScale;
		private UpdateTimeType _updateMode;
		private TweenLoopType _loopMode;
        private bool _shouldNodeBeCleaned = false;

		#region Constructor
		/// <summary>
		/// Constructor of TweenNode. Used internally by the Tween class to create the tween, do not use it.
		/// </summary>
		/// <remarks>
		/// Constructor of TweenNode. Used internally by the Tween class to create the tween, do not use it. To create TweenNode use the Tween class.		
		/// </remarks>
		public TweenNode(float p_startValue, float p_finalValue, float p_duration, TweenLoopType p_loopMode, UpdateTimeType p_updateMode, bool p_useTimeScale, int p_id, Action<float> p_callbackUser, Action p_callbackUpdate, Action p_callbackAnticipate)
		{
			//Initialize Data
			_id = p_id;
			_startValue = p_startValue;
			_endValue = p_finalValue;
			_useTimeScale = p_useTimeScale;
			_updateMode = p_updateMode;
			_loopMode = p_loopMode;
			_duration = p_duration;
			currentTimeStep = 0f;

			//Initialize Info
			_delegatedCallback = p_callbackUser;
			_onUpdate = p_callbackUpdate;
			_onAnticipate = p_callbackAnticipate;

			//Initialize
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

		#region Public Action Data
		/// <summary>
		/// Action that is called when tweenNode has completed its iteration.
		/// </summary>
		/// <remarks>
		/// Action that is called when tweenNode has completed its iteration. Useful for chaining actions related to each others.
		/// 
		/// Example below:
		/// <code>
		/// 
		/// Vox.TweenNode __showSpriteNode = Vox.Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_alpha)
		/// {
		/// 	Color __newColor = p_renderer.color;
		/// 	__newColor.a = p_alpha;
		/// 	p_renderer.color = __newColor;
		/// });
		/// 
		/// __showSpriteNode.onFinish += delegate()
		/// {
		/// 	Vox.TweenNode __changeSpriteColor = Vox.Tween.ColorTo(Color.white, Color.red, 1f, EaseType.Linear, delegate(Color p_color) 
		/// 	{
		/// 		p_renderer.color = p_color;	
		/// 	}
		/// };
		/// 
		/// </code>
		/// 
		/// </remarks>
		public Action onFinish;
		#endregion

		#region Private Get-Only Action Data
		/// <summary>
		/// Callback function passed via the TweenFunction. Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// Callback function passed via the TweenFunction. Used for debug and get only.
		/// </remarks>
		public Action<float> delegatedCallback
		{
			get { return _delegatedCallback; }
		}
		#endregion
	
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
		/// void StartSphereAnimation(GameObejct p_sphere)
		/// {
		/// 	TweenNode __moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
		/// 	{
		/// 		p_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
		/// 	});
		/// 
		/// 	__moveSphereInCircle.tag = "GamePlayState";
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
		/// void StartSphereAnimation(GameObejct p_sphere)
		/// {
		/// 	TweenNode __moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
		/// 	{
		/// 		p_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
		/// 	});
		/// 
		/// 	__moveSphereInCircle.associatedObject = p_sphere;
		/// }
		//
		/// </code>
		/// 
		/// </remarks>
		public GameObject associatedObject;

		/// <summary>
		/// CurrentTimeStep is a value that ranges from 0 to the duration passed to the tween. Its the value used to get the current value based on the ease or curve passed..
		/// </summary>
		/// <remarks>
		/// CurrentTimeStep is a value that ranges from 0 to the duration passed to the tween. Its the value used to get the current value based on the ease or curve passed..
		/// You can normalize it to check the current percentage of completion of the tween (the normalized value) by dividing this by the tween node duration.
		/// 
		/// Example of a use of the currentTimeStep usage, where a tween that moves a ball also change the scale of it based on the current completion of the tween.  
		/// 
		/// <code>
		/// 
		/// void StartSphereAnimation(GameObejct p_sphere)
		/// {
		/// 	TweenNode __moveSphereNode = null;
		/// 
		/// 	__moveSphereNode = Tween.Vector3To(Vector3.down * 5, Vector3.up * 5, 3f, EaseType.CUBIC_IN, delegate(Vector3 p_value)
		/// 	{
		/// 		p_sphere.transform.position = p_value;
		/// 		p_sphere.transform.localScale = p_sphere.transform.localScale * (__moveSphereNode.duration/__moveSphereNode.currentTimeStep);
		/// 	});
		/// 
		/// }
		///
		/// </code>
		/// 
		/// </remarks>
		public float currentTimeStep;

		/// <summary>
		/// isPingPongGoingBack is a boolean which indicates, if the TweenLoopMode is PING_PONG, the state of the loop. 
		/// </summary>
		/// <remarks>
		/// isPingPongGoingBack is a boolean which indicates, if the TweenLoopMode is PING_PONG, the state of the loop. 
		/// If true it means the loop is being reversed, else it means that the tween is making the normal interpolation first setted.
		/// 
		/// Example of a isPingPongGoingBack property is showed here:
		/// 
		/// <code>
		/// public void ApplyForceToObjectInLoop(Rigidbody p_body, Vector3 p_startForce, Vector3 p_endForce)
		/// {
		/// 	Vox.TweenNode __forceTween = null;
		///  
		/// 	__forceTweenNode = Vox.Tween.Vector3To(__startPosition, p_position, 1f, EaseType.Linear, true, TweenLoopMode.PING_PONG, UpdateTimeType.FIXED_UPDATE, delegate(Vector3 p_force)
		/// 	{
		/// 		if(__forceTween.isPingPongGoingBack)
		///				p_body.AddForce(-p_force);
		/// 		else 
		///				p_body.AddForce(p_force);
		/// 	});
		/// }
		/// </code>
		/// 
		/// </remarks>
		public bool isPingPongGoingBack = false;
		#endregion

		#region Private Get-Only Data
		/// <summary>
		/// Unique id among others TweenNodes. Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// Unique id among others TweenNodes. Used for debug and get only.
		/// </remarks>
		public int id { get { return _id; } }

		/// <summary>
		/// Current state of tween node. Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// Current state of tween node. Used for debug and get only.
		/// </remarks>
		public NodeState currentState { get { return _currentState; } }

		/// <summary>
		/// Start value to be used in tween passed via Vox.Tween.FloatTo(). Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// Start value to be used in tween passed via Vox.Tween.FloatTo(). Used for debug and get only.
		/// </remarks>
		public float startValue { get { return _startValue; } }

		/// <summary>
		/// End value to be used in tween passed via Vox.Tween.FloatTo(). Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// End value to be used in tween passed via Vox.Tween.FloatTo(). Used for debug and get only.
		/// </remarks>
		public float endValue { get { return _endValue; } }

		/// <summary>
		/// Duration to be used in tween passed via Vox.Tween. Used for debug and get only.
		/// </summary>
		/// <remarks>
		/// Duration to be used in tween passed via Vox.Tween. Used for debug and get only.
		/// </remarks>
		public float duration { get { return _duration; } }

		/// <summary>
		/// Defines if the tween will use Unity TimeScale or ignore it. Get only.
		/// </summary>
		/// <remarks>
		/// Defines if the tween will use Unity TimeScale or ignore it. Get only.
		/// </remarks>
		public bool useTimeScale { get { return _useTimeScale; } }

		/// <summary>
		/// UpdateTimeType to be used in tween passed via Vox.Tween.  Defines the type of update of the game or if it has one. Get only.
		/// </summary>
		/// <remarks>
		/// UpdateTimeType to be used in tween passed via Vox.Tween. Defines the type of update of the game or if it has one. Get only.
		/// </remarks>
		public UpdateTimeType updateMode { get { return _updateMode; } }

		/// <summary>
		/// TweenLoopMode to be used in tween passed via Vox.Tween. Defines the type of loop of the game or if it has one. Get only.
		/// </summary>
		/// <remarks>
		/// TweenLoopMode to be used in tween passed via Vox.Tween. Defines the type of loop of the game or if it has one. Get only.
		/// </remarks>
		public TweenLoopType loopMode { get { return _loopMode; } }
		#endregion

		#region UpdateNode
		/// <summary>
		/// Used internally by the TweenModule class to update the tween, do not use it.
		/// </summary>
		/// <remarks>
		/// Used internally by the TweenModule class to update the tween, do not use it.
		/// </remarks>
		public void UpdateNode()
		{
			if (_onUpdate != null) 
				_onUpdate();
		}
		#endregion

		#region Node Control Functions
		/// <summary>
		/// Resume the tween to be update if previously paused.
		/// </summary>
		/// <remarks>
		/// Resume the tween to be update if previously paused.
		/// 
		/// <code>
		/// Vox.TweenNode _moveSphereInCircle = null;
		/// 
		/// void StartSphereAnimation()
		/// {
		/// 	_moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
		/// 	{
		/// 		_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
		/// 	});
		/// 
		/// 	_moveSphereInCircle.associatedObject = p_sphere;
		/// }
		/// 
		/// void PauseSphereAnimation()
		/// {
		/// 	_moveSphereInCircle.Pause();
		/// }
		/// 
		/// void ResumeSphereAnimation()
		/// {
		/// 	_moveSphereInCircle.Resume();
		/// }
		/// </code>		
		/// </remarks>
		public void Resume()
		{
			_currentState = NodeState.PLAYING;
		}

		/// <summary>
		/// Pause the tween if running.
		/// </summary>
		/// <remarks>
		/// Pause the tween from being update if was playing.
		/// 
		/// <code>
		/// Vox.TweenNode _moveSphereInCircle = null;
		/// 
		/// void StartSphereAnimation()
		/// {
		/// 	_moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
		/// 	{
		/// 		_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
		/// 	});
		/// 
		/// }
		/// 
		/// void PauseSphereAnimation()
		/// {
		/// 	_moveSphereInCircle.Pause();
		/// }
		/// 
		/// void ResumeSphereAnimation()
		/// {
		/// 	_moveSphereInCircle.Resume();
		/// }
		/// </code>		
		/// </remarks>
		public void Pause()
		{
			ChangeState (NodeState.PAUSED);
		}

		/// <summary>
		/// Cancel the tween not calling the onFinish callback if it has one.
		/// </summary>
		/// <remarks>
		/// Cancel the tween not calling the onFinish callback if it has one.
		/// 
		/// <code>
		/// Vox.TweenNode _moveSphereInCircle = null;
		/// 
		/// void StartSphereAnimation()
		/// {
		/// 	_moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
		/// 	{
		/// 		_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
		/// 	});
		/// 
		/// 
		/// 	_moveSphereInCircle.onFinish += delegate()
		/// 	{
		/// 		Debug.Log("Will not be called if cancelled");
		/// 	};
		/// 
		/// 	_moveSphereInCircle.Cancel();
		/// }
		/// </code>	 	
		/// </remarks>
		public void Cancel()
		{
			_onUpdate = null;
			_onAnticipate = null;
			onFinish = null;
            _shouldNodeBeCleaned = true;
        }

		/// <summary>
		/// Make the tween go to the final value and call onFinish callback.
		/// </summary>
		/// <remarks>
		/// Make the tween go to the final value and call onFinish callback.
		/// 
		/// <code>
		/// Vox.TweenNode _moveSphereInCircle = null;
		/// 
		/// void StartSphereAnimation()
		/// {
		/// 	_moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
		/// 	{
		/// 		_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
		/// 	});
		/// 
		/// 
		/// 	_moveSphereInCircle.onFinish += delegate()
		/// 	{
		/// 		Debug.Log("Will be called if anticipated");
		/// 	};
		/// 
		/// 	_moveSphereInCircle.Anticipate(); // The update callback will be called one last time making the _sphere position to match the end value of the tween.
		/// }
		/// </code>	
		/// </remarks>
		public void Anticipate()
		{
			if(_onAnticipate != null)
				_onAnticipate ();
		}

        public bool ShouldNodeBeCleaned()
        {
            return _shouldNodeBeCleaned;
        }
        #endregion
    }
}