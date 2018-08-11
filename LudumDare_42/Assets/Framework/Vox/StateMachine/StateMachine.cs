#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Vox
{
	/// <summary>
	/// A generic statemachine class to be used in logic which is controlled by states.
	/// </summary>
	/// <remarks>
	/// A generic statemachine class to be used in logic which is controlled by states.
	///	Handing in a lot of useful features to change states, update them, default events and others.
	/// 
	/// Example of a class that inherits from the StateMachine, and of scene that uses it.
	/// 
	/// <code>
	/// //Code of the statemachine
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
	/// </code>
	/// 
	/// <code>
	/// // Code of a scene that has the statemachine.
	/// using UnityEngine;
	/// using System.Collections;
	/// using Vox;
	/// 
	/// public class GameScene : Scene
	/// {
	/// 	[SerializeField] private GameHUDStateMachine _HUDstateMachine;
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
	/// 			Debug.Log("Start Game!");
	/// 		}; 
	/// 	}
	///  
	/// 	public override void SceneUpdate ()
	/// 	{
	/// 	 	base.SceneUpdate ();
	/// 	
	/// 		_HUDstateMachine.StateMachineUpdate();
	/// 	}
	/// }
	/// </code>
	/// 
	/// </remarks> 
	[Serializable]
	public abstract class StateMachine<T> : MonoBehaviour
	{
		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		#region Private Atributtes

		[SerializeField] private T _startingStateType;
		private Dictionary<T, State<T>> _dictStates = new Dictionary<T, State<T>>();
		private T _activatedStateType;

		#endregion

		#region Private Methods

		private void ListenStateEvents(State<T> p_state)
		{
            p_state.onRequestChangeToState -= HandleStateOnRequestChangeToState;
			p_state.onRequestChangeToState += HandleStateOnRequestChangeToState;
		}

		private void HandleStateOnRequestChangeToState(T p_fromState, T p_toState, object[] p_args)
		{
			ChangeToState (p_toState, p_args);
		}

		#endregion

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region Public Data
		/// <summary>
		/// Set this to set activte or deactivate state objects automatically.
		/// </summary>
		/// <remarks>
		/// Set this to set activate or deactivate state objects automatically.
		/// </remarks>
		public bool activateAndDeactivateStatesOnEnableDisable = true;

		/// <summary>
		/// State that will start enabled in the Initialize.
		/// </summary>
		/// <remarks>
		/// State that will start enabled in the Initialize. Available to be set via inspector.
		/// </remarks>
		public T startingStateType
		{
			get{return _startingStateType;}
		}
		#endregion

		#region Initialize States
		/// <summary>
		/// <STRONG>(3 overloads)</STRONG> Function to call to Initialize StateMachine.
		/// </summary>
		/// <remarks>
		/// Function to call to Initialize StateMachine. Enabling states and calling their initialize functions.
		/// 
		/// <STRONG><em>void StateMachineInitialize(params object[] p_startingStatesArgs)</em></STRONG>
		/// 
		/// <STRONG><em>void StateMachineInitialize(T p_overrideStartingState, params object[] p_startingStatesArgs)</em></STRONG>
		///
		/// <STRONG><em>void StateMachineInitialize(List<T> p_overrideStartingStates, params object[] p_startingStatesArgs)</em></STRONG>
		///
		/// </remarks>
		public virtual void StateMachineInitialize(params object[] p_startingStatesArgs)
		{
			State<T>[] __listStatesFound = transform.GetComponentsInChildren<State<T>>(true);

			for (var i = 0; i < __listStatesFound.Length; i ++)
			{
				if (_dictStates.ContainsKey(__listStatesFound[i].stateType) == false)
				{
					_dictStates.Add(__listStatesFound[i].stateType, __listStatesFound[i]);
				}
				else
				{
					Debug.LogError("You cannot have more than one GameObject with the same State Type: " + __listStatesFound[i].stateType);
				}
            
				__listStatesFound[i].StateInitialize();

				if(activateAndDeactivateStatesOnEnableDisable)
					__listStatesFound[i].gameObject.SetActive (false);

				ListenStateEvents(__listStatesFound[i]);
			}

			_activatedStateType = _startingStateType;

			if(activateAndDeactivateStatesOnEnableDisable)
				_dictStates[_activatedStateType].gameObject.SetActive (true);

            _dictStates[_activatedStateType].StateOnEnable(_activatedStateType, p_startingStatesArgs);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif
		/// <summary>
		/// Function to call to Initialize StateMachine.
		/// </summary>
		/// <remarks>
		/// Function to call to Initialize StateMachine. Enabling states and calling their initialize functions.
		/// </remarks>
		public virtual void StateMachineInitialize(T p_overrideStartingState, params object[] p_startingStatesArgs)
		{
			_startingStateType = p_overrideStartingState;

			StateMachineInitialize(p_startingStatesArgs);
		}
			
		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region Public control methods
		/// <summary>
		/// Enable an state and disable all others.
		/// </summary>
		/// <remarks>
		/// Function to enable an state and disable all others.
		/// </remarks>		
		public State<T> ChangeToState(T p_toState, params object[] p_args)
		{
			if (_activatedStateType.ToString() != p_toState.ToString())
			{
                T __previousState = _activatedStateType;
                _activatedStateType = p_toState;

                if (__previousState != null)
                {
                    _dictStates[__previousState].StateOnDisable( );

                    if (activateAndDeactivateStatesOnEnableDisable)
                    {
                        _dictStates[__previousState].gameObject.SetActive( false );
                    }
                }
                else
                {
                    __previousState = _activatedStateType;
                }

				if (activateAndDeactivateStatesOnEnableDisable) 
				{
					_dictStates [_activatedStateType].gameObject.SetActive (true);
				}

                _dictStates[_activatedStateType].StateOnEnable(__previousState, p_args);
			}

			return _dictStates[p_toState];
		}

		/// <summary>
		/// Returns the required State of the StateMachine.
		/// </summary>
		/// <remarks>
		/// Returns the required State of the StateMachine.
		/// </remarks>	
		public State<T> GetState(T p_stateToGet)
		{
			return _dictStates[p_stateToGet];
		}

		/// <summary>
		/// Returns the required State of the StateMachine.
		/// </summary>
		/// <remarks>
		/// Returns the required State of the StateMachine.
		/// </remarks>	
		public T GetActivatedStateType()
		{
			return _activatedStateType;
		}


		#endregion

		#region Update Methods
		/// <summary>
		/// Update all the states in the State Machine.
		/// </summary>
		/// <remarks>
		/// Update all the states in the State Machine. To be implemented on Vox.Scene.SceneUpdate();
		/// </remarks>
		public virtual void StateMachineUpdate()
		{
			if(_activatedStateType != null)
				_dictStates[_activatedStateType].StateUpdate();
		}

		/// <summary>
		///  Fixed Update all the states in the State Machine.
		/// </summary>
		/// <remarks>
		/// Fixed Update all the states in the State Machine. To be implemented on Vox.Scene.SceneFixedUpdate();
		/// </remarks>
		public virtual void StateMachineFixedUpdate()
		{
			if(_activatedStateType != null)
				_dictStates[_activatedStateType].StateFixedUpdate();
		}

		/// <summary>
		/// Late Update all the states in the State Machine.
		/// </summary>
		/// <remarks>
		/// Late Update all the states in the State Machine. To be implemented on Vox.Scene.SceneLateUpdate();
		/// </remarks>
		public virtual void StateMachineLateUpdate()
		{
			if(_activatedStateType != null)
				_dictStates[_activatedStateType].StateLateUpdate();
		}

		/// <summary>
		/// Call Vox.State.StateOnGUI of all the states in the State Machine.
		/// </summary>
		/// <remarks>
		/// Call Vox.State.StateOnGUI of all the states in the State Machine. To be implemented on Vox.Scene.SceneOnGUI();
		/// </remarks>
		public virtual void StateMachineOnGUI()
		{
			if(_activatedStateType != null)
				_dictStates[_activatedStateType].StateOnGUI();
		}

		/// <summary>
		/// Call Vox.State.StateOnUnloadedScene of all the states in the State Machine.
		/// </summary>
		/// <remarks>
		/// Call Vox.State.StateOnUnloadedScene of all the states in the State Machine. To be implemented on Vox.Scene.OnUnloadedScene();
		/// </remarks>
		public virtual void StateMachineOnUnloadedScene()
		{ 
			if(_activatedStateType != null)
				_dictStates[_activatedStateType].StateOnUnloadedScene();
		}
		#endregion

		#region OnApplication Methods
		/// <summary>
		/// Call Vox.State.StateOnApplicationFocused of all the states in the State Machine.
		/// </summary>
		/// <remarks>
		/// Call Vox.State.StateOnApplicationFocused of all the states in the State Machine. To be implemented on Vox.Scene.SceneOnApplicationFocused();
		/// </remarks>
		public virtual void StateMachineOnApplicationFocused(bool p_focusStatus)
		{
			if(_activatedStateType != null)
				_dictStates[_activatedStateType].StateOnApplicationFocused(p_focusStatus);
		}

		/// <summary>
		/// Call OnApplicationPaused of all the states in the State Machine.
		/// </summary>
		/// <remarks>
		/// Call OnApplicationPaused of all the states in the State Machine. To be implemented on Vox.Scene.SceneOnApplicationPaused();
		/// </remarks>
		public virtual void StateMachineOnApplicationPaused(bool p_pausedStatus)
		{
			if(_activatedStateType != null)
				_dictStates[_activatedStateType].StateOnApplicationPaused(p_pausedStatus);
		}

		/// <summary>
		/// Call Vox.State.StateOnQuitApplication of all the states in the State Machine.
		/// </summary>
		/// <remarks>
		/// Call Vox.State.StateOnQuitApplication of all the states in the State Machine. To be implemented on Vox.Scene.SceneOnQuitApplication();
		/// </remarks> 
		public virtual void StateMachineOnQuitApplication()
		{ 
			if(_activatedStateType != null)
				_dictStates[_activatedStateType].StateOnQuitApplication();
		}
		#endregion
	}
}