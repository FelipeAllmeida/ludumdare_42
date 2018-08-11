using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace VoxInternal
{
	/// <summary>
	/// Class which has OnCollisionStay checking event.
	/// </summary>
	public class CollisionStayDetector : MonoBehaviour  
	{
		public Action<Collision> onCollisionStay;

		/// <summary>
		/// OnCollisionStay checking event from unity.
		/// </summary>
		void OnCollisionStay(Collision p_collision)	
		{
			if(onCollisionStay != null)
			{
				onCollisionStay(p_collision);
			}
		}
	}
}