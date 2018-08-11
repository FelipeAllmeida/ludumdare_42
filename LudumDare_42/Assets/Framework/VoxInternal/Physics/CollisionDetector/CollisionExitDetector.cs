using UnityEngine;
using System.Collections;
using System;

namespace VoxInternal
{
	/// <summary>
	/// Class which has OnCollisionExit checking event.
	/// </summary>
	public class CollisionExitDetector : MonoBehaviour 
	{
		public Action<Collision> onCollisionExit;
		 
		/// <summary>
		/// OnCollisionExit checking event from unity.
		/// </summary>
		void OnCollisionExit(Collision p_collision)	
		{
			if(onCollisionExit != null)
			{
				onCollisionExit(p_collision);
			}
		}
	}
}