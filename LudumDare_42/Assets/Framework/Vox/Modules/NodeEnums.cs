using UnityEngine;
using System;
using System.Collections;

namespace Vox
{
	/// <summary>
	/// Type of time used in update method to be used by node.
	/// </summary>
	/// <remarks>
	/// Type of update method used by node. They are the default loop events of unity.
	/// </remarks> 
	public enum UpdateTimeType
	{
		UPDATE,
		FIXED_UPDATE,
		LATE_UPDATE,
		GUI
	}
		
	/// <summary>
	/// Current State of the node.
	/// </summary>
	/// <remarks>
	/// Current State of the node. Playing or paused.
	/// </remarks> 
	public enum NodeState
	{
		PLAYING,
		PAUSED,
        FINISHED
	}
}

