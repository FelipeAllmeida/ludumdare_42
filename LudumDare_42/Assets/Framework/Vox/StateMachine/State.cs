#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Vox
{
	/// <summary>
	/// A state of the Vox.StateMachine.
	/// </summary>
	/// <remarks>
	/// A state of the Vox.StateMachine. Has several features that integrates it with the rest of the state machine, having enable and disable callbacks, passing parameters to other states and others. <BR>
	/// 
	/// Example of a state and its statemachine:
	/// 
	/// <code>
	/// // Code of the state
	/// using System;
	/// using System.Collections;
	/// using UnityEngine;
	/// using UnityEngine.UI;
	/// using Vox;
	/// 
	/// public class CountdownhudState : State<GameHUDStateMachine.GameHUDStates>
	/// {
	/// 	[SerializeField] private GameObject _canvas;
	/// 
	/// 	[SerializeField] private Text _countdownText;
	/// 
	/// 	public Action onFinishedCountdown;
	/// 
	/// 	public override void StateInitialize ()
	/// 	{
	/// 		_canvas.gameObject.SetActive (false);
	/// 	}
	/// 
	/// 	public override void StateOnEnable (GameHUDStateMachine.GameHUDStates p_fromStateType, params object[] p_enableParameter)
	/// 	{
	/// 		_canvas.gameObject.SetActive (true);
	/// 		StartGameCountdown (3);
	/// 	}
	/// 
	/// 	public override void StateOnDisable ()
	/// 	{
	/// 		_canvas.gameObject.SetActive (false);
	/// 	}
	/// 
	/// 	private void StartGameCountdown(int p_countdownTime)
	/// 	{
	/// 		_countdownText.enabled = true;
	/// 
	/// 		TimerNode __node = null;
	/// 
	/// 		__node = Timer.WaitSeconds (1, true, p_countdownTime, delegate()
	/// 		{
	/// 			_countdownText.text = (p_countdownTime - __node.loopIteration).ToString();
	/// 
	/// 			if(p_countdownTime - __node.loopIteration == 0)
	/// 			{
	/// 				_countdownText.enabled = false;
	/// 
	/// 				if(onFinishedCountdown != null)
	/// 					onFinishedCountdown();
	/// 			}
	/// 		});
	/// 	}
	/// } 
	/// </code>
	/// 
	/// <code>
	/// // Code of the statemachine
	/// using UnityEngine;
	/// using System;
	/// using System.Collections;
	/// using Vox;
	/// 
	/// public class GameHUDStateMachine : StateMachine<GameHUDStateMachine.GameHUDStates> 
	/// {
	/// 	public enum GameHUDStates
	/// 	{
	/// 		GAMEPLAY,
	/// 		PAUSED,
	/// 		GAMEOVER,
	/// 		COUNTDOWN
	/// 	};
	/// 
	/// 	public Action onFinishedStartCountdown;
	/// 
	/// 	public void InitializeStatesEvents()
	/// 	{
	/// 		((CountdownhudState)GetState (GameHUDStates.COUNTDOWN)).onFinishedCountdown += delegate()
	/// 		{
	/// 			if(onFinishedStartCountdown != null)
	/// 				onFinishedStartCountdown();
	/// 		};
	/// 	}
	/// }
	/// 
	/// </code>
	/// 
	/// </remarks>	 
	[Serializable]
	public abstract class State<T> : MonoBehaviour
	{
		#region Private Internal Only

		#region Inspector Data

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif


		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#endregion

		#region Event Data
		/// <summary>
		/// Internal action used to change state. Use Vox.State.ChangeToState() instead.
		/// </summary>
		/// <remarks>
		/// Internal action used to change state. Use Vox.State.ChangeToState() instead.
		/// </remarks>	 
		public event Action<T, T, object[]> onRequestChangeToState;
		#endregion

		#region Public Get-Only Data
		/// <summary>
		/// Type of the state. Has to be set in inspector.
		/// <remarks>
		/// Type of the state. Has to be set in inspector.
		/// Comes from the enum created to be used in the statemachine.
		/// Is get only.
		/// </remarks>	
		public T stateType;
		#endregion

		#region Virtual Methods
		/// <summary>
		/// Create the initialization logic of the state here.
		/// <remarks>
		/// Create the initialization logic of the state here.
		/// Will be called by Vox.StateMachine.StateMachineInitialize();
		/// </remarks>	
		public virtual void StateInitialize()
		{
		}

		/// <summary>
		/// Called when manager enable this state, may received adtional params.
		/// </summary>
		/// <remarks>
		/// Called when manager enable this state, may received adtional params.
		/// Use this to activate required features to be available from the activation of the state.
		/// </remarks>	 
		public virtual void StateOnEnable(T p_fromStateType, params object[] p_enableParameter)
		{

		}
		/// <summary>
		/// Called when manager disable this state.
		/// </summary>
		/// <remarks>
		/// Called when manager disable this state.
		/// Use this to deactivate required features that are related only to this event.
		/// </remarks>	 
		public virtual void StateOnDisable()
		{

		}
		/// <summary>
		/// Called when manager disable this state.
		/// </summary>
		/// <remarks>
		/// Called when manager disable this state.
		/// Use this to deactivate required features that are related only to this event.
		/// </remarks>	 
		public virtual void StateUpdate()
		{ 

		}

		/// <summary>
		/// Called every physics update.
		/// </summary>
		/// <remarks>
		/// It can be called once every 2 or 3 frames.
		/// Will be called by Vox.StateMachine.StateMachineFixedUpdate();
		/// Use this method only with physics modifications, like changing a rigidbody or a object with a collider.
		/// </remarks>
		public virtual void StateFixedUpdate()
		{ 

		}

		/// <summary>
		/// Called after every frame update.
		/// </summary>
		/// <remarks>
		/// Called after every frame update.
		/// Will be called by Vox.StateMachine.StateMachineLateUpdate();
		/// </remarks>
		public virtual void StateLateUpdate()
		{ 

		}

		/// <summary>
		/// Called on every GUI event.
		/// </summary>
		/// <remarks>
		/// Note that this method can be called more than once per frame.
		/// Will be called by Vox.StateMachine.StateMachineOnGUI();
		/// </remarks>		
		public virtual void StateOnGUI()
		{ 

		}

		/// <summary>
		/// Called on Apllication got focused.
		/// </summary>
		/// <remarks>
		/// Called on Aplication got focused.
		/// Called by Vox.StateMachine.StateMachineOnApplicationFocused, implemented on Vox.Scene.SceneOnApplicationFocused();
		/// </remarks>
		public virtual void StateOnApplicationFocused(bool p_focusStatus)
		{

		}
		/// <summary>
		/// Called on Apllication lost focus.
		/// </summary>
		/// <remarks>
		/// Called on Aplication lost focus. 
		/// Called by Vox.StateMachine.StateMachineOnApplicationPaused, implemented on Vox.Scene.SceneOnApplicationPaused();
		/// </remarks> 
		public virtual void StateOnApplicationPaused(bool p_pausedStatus)
		{

		}

		/// <summary>
		/// Called on Quit event.
		/// </summary>
		/// <remarks>
		/// Called on Aplication lost focus. 
		/// Called by Vox.StateMachine.StateMachineOnApplicationPaused, implemented on Vox.Scene.SceneOnApplicationPaused();
		/// </remarks>  
		public virtual void StateOnQuitApplication()
		{ 

		}

		/// <summary>
		/// Called on Unloaded Scene.
		/// </summary>
		/// <remarks>
		/// Called on Aplication lost focus. 
		/// Called by Vox.StateMachine.StateMachineOnApplicationPaused, implemented on Vox.Scene.SceneOnApplicationPaused();
		/// </remarks>  
		public virtual void StateOnUnloadedScene()
		{ 

		}
		#endregion

		#region Protected Methods

		/// <summary>
		/// Change from this state to another, returning the activated state.
		/// </summary>
		/// <remarks>
		/// Change from this state to another, returning the activated state.
		/// </remarks>  
		protected void ChangeToState(T p_stateType, params object[] p_args)
		{
			if (onRequestChangeToState != null) 
				onRequestChangeToState(stateType, p_stateType, p_args);
		}
		#endregion
	}
}