using UnityEngine;
using System.Collections;
using System;

namespace VoxInternal
{
	/// <summary>
	/// Class which has OnTriggerEnter2D checking event.
	/// </summary>
	public class TriggerEnterDetector2D : MonoBehaviour 
	{
		public Action<Collider2D> onTriggerEnter;

		/// <summary>
		/// OnTriggerEnter2D checking event from unity.
		/// </summary>
		void OnTriggerEnter2D(Collider2D p_collider)
		{

			if(onTriggerEnter != null)
			{
				onTriggerEnter(p_collider);
			}
		}
	}

}
