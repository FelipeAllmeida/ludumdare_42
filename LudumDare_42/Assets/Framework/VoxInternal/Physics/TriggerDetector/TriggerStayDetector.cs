using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxInternal
{
	/// <summary>
	/// Class which has OnTriggerStay checking event.
	/// </summary>
	public class TriggerStayDetector : MonoBehaviour 
	{   
		public Action<Collider> onTriggerStay;	

		/// <summary>
		/// OnTriggerStay checking event from unity.
		/// </summary>
		void OnTriggerStay(Collider p_collider)	
		{
			if(onTriggerStay != null)
			{
				onTriggerStay(p_collider);
			}
		}
	}
}