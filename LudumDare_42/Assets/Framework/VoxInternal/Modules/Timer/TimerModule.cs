using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vox;

namespace VoxInternal
{
	/// <summary>
	/// Module which updates Timer functions from a dictionary of TimerNodes.
	/// </summary>
	public class TimerModule : Module
	{
		#region Public Static Data
		/// <summary>
		/// Create automatically the unique instance of this class.
		/// </summary>
		static private TimerModule _instance;
		static public TimerModule instance
		{
			get 
			{ 
				if (_instance == null)
				{
					_instance = InstanceInitialize ();
				}

				return _instance;
			}
		}
		#endregion

		#region Private Data
		private List<TimerNode> _listOfTimers = new List<TimerNode>();

		#endregion
		
		#region Singleton Instance Initialization
		/// <summary>
		/// Handles the logic required in the inicialization of the instance.
		/// </summary>
		static private TimerModule InstanceInitialize()
		{
			TimerModule __instance = new TimerModule ();
			ModuleCore.instance.AddModule(__instance);

			return __instance;
		}
		#endregion

		#region NodeControl
		/// <summary>
		/// Function to be called from a TimerNode to add itself to this module dictionary.
		/// </summary>
		public void AddNode(TimerNode p_node)
		{
			_listOfTimers.Add(p_node);
		}

		/// <summary>
		/// Clear all nodes in the current dictionary
		/// </summary>
		public override void ClearAllNodes()
		{
			foreach (TimerNode __node in _listOfTimers)
			{
				__node.Cancel();
			}

			_listOfTimers.Clear();
		}

		/// <summary>
		/// Clear all nodes in the current dictionary
		/// </summary>
		public override void ClearNodesWithTag(string p_tag)
		{
			for (int i = 0; i < _listOfTimers.Count; i++)
			{
				if (_listOfTimers [i].tag == p_tag)
				{
					_listOfTimers[i].Cancel();

					if (_listOfTimers.Count - 1 >= i)
				    {
						_listOfTimers.RemoveAt( i );
				        i--;
				    }
				}
			}
		}
		/// <summary>
		/// Clear all nodes in the current dictionary except for the nodes with the passed tag
		/// </summary>
		public override void ClearNodesExceptForTag(string p_tag)
		{
			for (int i = 0; i < _listOfTimers.Count; i++)
			{
				if (_listOfTimers [i].tag != p_tag)
				{
					_listOfTimers[i].Cancel();

					if (_listOfTimers.Count - 1 >= i)
				    {
						_listOfTimers.RemoveAt( i );
				        i--;
				    }
                }
			}
		}
		#endregion

		#region UpdateNodes
		/// <summary>
		/// Update all active TimerNode in  in the nodes dictionary.
		/// </summary>
		public override void ModuleUpdate()
		{
			for (int i = 0; i < _listOfTimers.Count; i++)
			{
				if (_listOfTimers [i].updateMode != UpdateTimeType.LATE_UPDATE)
				{
					_listOfTimers [i].UpdateNode ();

					if (_listOfTimers[i].ShouldNodeBeCleaned())
					{
						_listOfTimers[i].Cancel();
						_listOfTimers.RemoveAt( i );
						i--;
					}
				}
			}
		}

		/// <summary>
		/// Update all active TimerNode in  in the nodes dictionary.
		/// </summary>
		public override void ModuleLateUpdate()
		{
			for (int i = 0; i < _listOfTimers.Count; i++)
			{
				if (_listOfTimers [i].updateMode == UpdateTimeType.LATE_UPDATE)
				{
					_listOfTimers [i].UpdateNode ();

					if (_listOfTimers[i].ShouldNodeBeCleaned())
					{
						_listOfTimers[i].Cancel();
						_listOfTimers.RemoveAt( i );
						i--;
					}
				}
			}
		}
		#endregion
	}
}