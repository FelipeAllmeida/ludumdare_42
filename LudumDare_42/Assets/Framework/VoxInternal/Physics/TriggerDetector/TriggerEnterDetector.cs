using UnityEngine;
using System;
using System.Collections;

namespace VoxInternal
{
	/// <summary>
	/// Class which has OnTriggerEnter checking event.
	/// </summary>
	public class TriggerEnterDetector : MonoBehaviour 
	{
		public Action<Collider> onTriggerEnter;

		/// <summary>
		/// OnTriggerEnter checking event from unity.
		/// </summary>
		void OnTriggerEnter(Collider p_collider)	
		{
			if(onTriggerEnter != null)
			{
				onTriggerEnter(p_collider);
			}
		}
	}
}
