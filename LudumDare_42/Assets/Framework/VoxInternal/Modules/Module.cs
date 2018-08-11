using UnityEngine;
using System.Collections;

namespace VoxInternal
{

	/// <summary>
	/// Base class for framework Modules.
	/// </summary>
	[System.Serializable]
	public class Module
	{
		protected int _nextNodeID = 0;

		#region Node Control
		/// <summary>
		/// Function to manage id of added nodes.
		/// </summary>
		public virtual int GetNextNodeID()
		{
			int __id = _nextNodeID;
			_nextNodeID++;
			return __id;
		}

		/// <summary>
		/// Clear all nodes of the module.
		/// </summary>
		public virtual void ClearAllNodes()
		{

		}

		/// <summary>
		/// Clear all nodes with the passed tag of the module.
		/// </summary>
		public virtual void ClearNodesWithTag(string p_tag)
		{

		}

		/// <summary>
		/// Clear all nodes except for the ones with the passed tag of the module.
		/// </summary>
		public virtual void ClearNodesExceptForTag(string p_tag)
		{

		}

		#endregion

		#region Update Virtual Functions
		/// <summary>
		/// Update module.
		/// </summary>
		public virtual void ModuleUpdate()
		{

		}

		/// <summary>
		/// Update module after other updates.
		/// </summary>
		public virtual void ModuleLateUpdate()
		{	

		}

		/// <summary>
		/// Update module using fixed time.
		/// </summary>
		public virtual void ModuleFixedUpdate()
		{

		}

		/// <summary>
		/// Draw module GUI.
		/// </summary>
		public virtual void ModuleOnGUI()
		{
			
		}
		#endregion
	}
}