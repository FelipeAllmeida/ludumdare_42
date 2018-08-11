using UnityEngine;
using System;
using System.Collections;

namespace VoxInternal
{
	/// <summary>
	/// Class which has OnCollisionEnter checking event.
	/// </summary>
	public class CollisionEnterDetector : MonoBehaviour 
	{
		public event Action<Collision> onCollisionEnter;

		/// <summary>
		/// OnCollisionEnter checking event from unity.
		/// </summary>
		void OnCollisionEnter(Collision p_collision)	
		{
			if(onCollisionEnter != null)
			{
				onCollisionEnter(p_collision);
			}
		}
	}
}
