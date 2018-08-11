#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System;
using System.Collections;
using VoxInternal;

namespace Vox
{
	/// <summary>
	/// Class to be used to get collision events and add methods to them outside.
	/// </summary>
	/// <remarks>
	/// Useful to not have to set a collision detector on the same gameobject of a script that controls that collision check.  <BR>
	/// Example: A manager of a race track can have the references to the track collision triggers, but this triggers can be in GameObjects such as flags, boxes and others in the scenery and still link its events to the desired script.
	/// To optimize performance select which events you want to check in the inspector.
	/// 
	/// For more information check: <BR>
	/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnCollisionEnter.html"><STRONG>Unity Collision Enter</STRONG></A><BR>
	/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnCollisionExit.html"><STRONG>Unity Collision Exit</STRONG></A><BR>
	/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnCollisionStay.html"><STRONG>Unity Collision Stay</STRONG></A>
	///
	/// </remarks>
	public class CollisionDetector : PhysicsDetector 
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

		private CollisionEnterDetector _collisionEnterDetector;
		private CollisionExitDetector _collisionExitDetector;
		private CollisionStayDetector _collisionStayDetector;


		/// <summary>
		/// Unity default awake to initialize events checked in inspector.
		/// </summary>
		private void Awake () 
		{
			//Enter detection
			if (detectEnter) 
			{
				_collisionEnterDetector = gameObject.AddComponent<CollisionEnterDetector> ();

				_collisionEnterDetector.onCollisionEnter += delegate(Collision p_collision) 
				{
					_isTriggered = true;

					if (onCollisionEnter != null)
						onCollisionEnter (p_collision);
				};
			}
			else if (Application.isEditor && _debugVisuallyOnlyListeningEvents == false) 
			{
				_collisionEnterDetector = gameObject.AddComponent<CollisionEnterDetector> ();

				_collisionEnterDetector.onCollisionEnter += delegate(Collision p_collision) 
				{
					_isTriggered = true;
				};
			}	

			//Exit detection
			if (detectExit) 
			{
				_collisionExitDetector = gameObject.AddComponent<CollisionExitDetector>();

				_collisionExitDetector.onCollisionExit+= delegate(Collision p_collision) 
				{
					_isTriggered = false;

					if(onCollisionExit != null)
						onCollisionExit(p_collision);
				};
			}
			else if (Application.isEditor && _debugVisuallyOnlyListeningEvents == false) 
			{
				_collisionExitDetector = gameObject.AddComponent<CollisionExitDetector>();

				_collisionExitDetector.onCollisionExit+= delegate(Collision p_collision) 
				{
					_isTriggered = false;
				};
			}

			//Stay detection
			if (detectStay) 
			{
				_collisionStayDetector = gameObject.AddComponent<CollisionStayDetector>();

				_collisionStayDetector.onCollisionStay += delegate(Collision p_collision) 
				{
					_isTriggered = true;

					if(onCollisionStay != null)
						onCollisionStay(p_collision);
				};
			}
			else if (Application.isEditor && _debugVisuallyOnlyListeningEvents == false) 
			{
				_collisionStayDetector = gameObject.AddComponent<CollisionStayDetector>();

				_collisionStayDetector.onCollisionStay += delegate(Collision p_collision) 
				{
					_isTriggered = true;
				};
			}	
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		/// <summary>
		/// Action called when Unity default onColissionEnter happens.
		/// </summary>
		/// <remarks>
		/// Action called when Unity default onColissionEnter happens. Use it as you would use the default Unity Function.
		/// For more information check: <BR>
		/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnCollisionEnter.html"><STRONG>Unity Collision Enter</STRONG></A><BR>
		/// </remarks>
		public Action<Collision> onCollisionEnter = null;

		/// <summary>
		/// Action called when Unity default onColissionExit happens.
		/// </summary>
		/// <remarks>
		/// Action called when Unity default onColissionExit happens. Use it as you would use the default Unity Function.
		/// For more information check: <BR>
		/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnCollisionExit.html"><STRONG>Unity Collision Exit</STRONG></A><BR>
		/// </remarks>
		public Action<Collision> onCollisionExit = null;

		/// <summary>
		/// Action called when Unity default onColissionStay happens.
		/// </summary>
		/// <remarks>
		/// Action called when Unity default onColissionStay happens. Use it as you would use the default Unity Function.
		/// For more information check: <BR>
		/// - <A href="http://docs.unity3d.com/ScriptReference/Collider.OnCollisionStay.html"><STRONG>Unity Collision Stay</STRONG></A> 
		/// </remarks>
		public Action<Collision> onCollisionStay = null;

	}
}