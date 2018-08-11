using UnityEngine;
using System.Collections;
using System;

namespace VoxInternal
{
	/// <summary>
	/// Class which has OnTriggerExit checking event.
	/// </summary>
	public class TriggerExitDetector : MonoBehaviour 
	{
		public Action<Collider> onTriggerExit;

		/// <summary>
		/// OnTriggerExit checking event from unity.
		/// </summary>
		void OnTriggerExit(Collider p_collider)	
		{
			if(onTriggerExit != null)
			{
				onTriggerExit(p_collider);
			}
		}
	}
}