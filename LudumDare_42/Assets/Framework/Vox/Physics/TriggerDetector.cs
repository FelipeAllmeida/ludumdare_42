#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System;
using System.Collections;
using VoxInternal;

namespace Vox
{
	/// <summary>
	/// Class to be used to get trigger events and add methods to them outside.
	/// </summary>
	/// <remarks>
	/// Useful to not have to set a trigger detector on the same gameobject of a script that controls that trigger check. <BR>
	/// 
	/// Example: A manager of a race track can have the references to the track collision triggers, but this triggers can be in GameObjects such as flags, boxes and others in the scenery and still link its events to the desired script. <BR>
	/// 
	/// To optimize performance select which events you want to check in the inspector.
	/// 
	/// For more information check: <BR>
	/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html"><STRONG>Unity Trigger Enter</STRONG></A><BR>
	/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnTriggerExit.html"><STRONG>Unity Trigger Exit</STRONG></A><BR>
	/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnTriggerStay.html"><STRONG>Unity Trigger Stay</STRONG></A>
	/// 
	/// </remarks>
	public class TriggerDetector : PhysicsDetector 
	{
		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		[Header ("Listened Events")]

		[SerializeField]
		private bool detectEnter = true;
		[SerializeField]
		private bool detectExit = true;
		[SerializeField]
		private bool detectStay = false;

		private TriggerEnterDetector _triggerEnterDetector;
		private TriggerExitDetector _triggerExitDetector;
		private TriggerStayDetector _triggerStayDetector;

		/// <summary>
		/// Unity default awake to initialize events checked in inspector.
		/// </summary>
		private void Awake () 
		{
			// Enter
			if (detectEnter) 
			{
				_triggerEnterDetector = gameObject.AddComponent<TriggerEnterDetector> ();

				_triggerEnterDetector.onTriggerEnter += delegate(Collider p_collider) 
				{
					_isTriggered = true;

					if (onTriggerEnter != null)
						onTriggerEnter (p_collider);
				};
			} 
			else if (Application.isEditor && _debugVisuallyOnlyListeningEvents == false) 
			{
				_triggerEnterDetector = gameObject.AddComponent<TriggerEnterDetector> ();

				_triggerEnterDetector.onTriggerEnter += delegate(Collider p_collider) 
				{
					_isTriggered = true;
				};
			}

			// Exit
			if (detectExit) 
			{
				_triggerExitDetector = gameObject.AddComponent<TriggerExitDetector>();

				_triggerExitDetector.onTriggerExit+= delegate(Collider p_collider) 
				{
					_isTriggered = false;

					if(onTriggerExit != null)
						onTriggerExit(p_collider);
				};
			}
			else if (Application.isEditor && _debugVisuallyOnlyListeningEvents == false) 
			{
				_triggerExitDetector = gameObject.AddComponent<TriggerExitDetector>();

				_triggerExitDetector.onTriggerExit+= delegate(Collider p_collider) 
				{
					_isTriggered = false;
				};
			}

			// Stay
			if (detectStay) 
			{
				_triggerStayDetector = gameObject.AddComponent<TriggerStayDetector> ();

				_triggerStayDetector.onTriggerStay += delegate(Collider p_collider) {

					_isTriggered = true;

					if (onTriggerStay != null)
						onTriggerStay (p_collider);
				};
			}
			else if (Application.isEditor && _debugVisuallyOnlyListeningEvents == false) 
			{
				_triggerStayDetector = gameObject.AddComponent<TriggerStayDetector> ();

				_triggerStayDetector.onTriggerStay += delegate(Collider p_collider) {

					_isTriggered = true;
				};
			}
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		/// <summary>
		/// Action called when Unity default onTriggerEnter happens.
		/// </summary>
		/// <remarks>
		/// Action called when Unity default onTriggerEnter happens. Use it as you would use the default Unity Function.
		/// For more information check: <BR>
		/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html"><STRONG>Unity Trigger Enter</STRONG></A>
		/// </remarks>
		public Action<Collider> onTriggerEnter = null;

		/// <summary>
		/// Action called when Unity default onTriggerExit happens. Use it as you would use the default Unity Function.
		/// </summary>
		/// <remarks>
		/// Action called when Unity default onTriggerExit happens.
		/// For more information check: <BR>
		/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnTriggerExit.html"><STRONG>Unity Trigger Exit</STRONG></A>
		/// </remarks>
		public Action<Collider> onTriggerExit = null;

		/// <summary>
		/// Action called when Unity default onTriggerStay happens. Use it as you would use the default Unity Function.
		/// </summary>
		/// <remarks>
		/// Action called when Unity default onTriggerStay happens.
		/// For more information check: <BR>
		/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnTriggerStay.html"><STRONG>Unity Trigger Stay</STRONG></A>
		/// </remarks>
		public Action<Collider> onTriggerStay = null;


	}
}