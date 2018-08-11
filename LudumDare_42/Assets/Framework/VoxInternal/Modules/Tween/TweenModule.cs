using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vox;


namespace VoxInternal
{	
	/// <summary>
	/// Module which updates Tween functions from a dictionary of TweenNodes.
	/// </summary>
	[System.Serializable]
	public class TweenModule : Module
	{
		#region Public Static Data
		/// <summary>
		/// Create automatically the unique instance of this class.
		/// </summary>
		static private TweenModule _instance;
		static public TweenModule instance
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

		#region Private Get-Only Data
	    private List<TweenNode> _listOfNodes = new List<TweenNode>();

		public List<TweenNode> listOfNodes
        {
			get { return _listOfNodes; }
		}
		#endregion


		#region Singleton Instance Initialization
		/// <summary>
		/// Handles the logic required in the inicialization of the instance.
		/// </summary>
		static private TweenModule InstanceInitialize()
		{
			TweenModule __instance = new TweenModule ();

			ModuleCore.instance.AddModule(__instance);

			return __instance;
		}
		#endregion

		#region UpdateNodes
		/// <summary>
		/// Update all active TweenNode in the nodes dictionary.
		/// </summary>
		public override void ModuleUpdate()
		{
		    UpdateNodesOfType( UpdateTimeType.UPDATE );
        }
        /// <summary>
        /// Update all active TweenNode in the nodes dictionary.
        /// </summary>
        public override void ModuleLateUpdate()
		{
		    UpdateNodesOfType( UpdateTimeType.LATE_UPDATE );

        }
        /// <summary>
        /// Update all active TweenNode  in the nodes dictionary.
        /// </summary>
        public override void ModuleFixedUpdate()
		{
		    UpdateNodesOfType( UpdateTimeType.FIXED_UPDATE );
        }
        /// <summary>
        /// Update all active TweenNode in the nodes dictionary.
        /// </summary>
        public override void ModuleOnGUI()
		{
		    UpdateNodesOfType(UpdateTimeType.GUI);
		}

        private void UpdateNodesOfType(UpdateTimeType p_type)
        {
            for (int i = 0; i < _listOfNodes.Count; i++)
            {
                if (_listOfNodes[i].updateMode == p_type)
                {
                    if (_listOfNodes[i].ShouldNodeBeCleaned())
                    {
                        _listOfNodes[i] = null;
                        _listOfNodes.RemoveAt( i );
                        i--;
                        continue;
                    }
                    _listOfNodes[i].UpdateNode();
                }
            }
        }
		#endregion

		#region NodeControl
		/// <summary>
		/// Function to be called from a TweenNode to add itself to this module dictionary.
		/// </summary>
		public void AddNode(TweenNode p_node, UpdateTimeType p_updateMode)
		{
		    _listOfNodes.Add(p_node);
		}

		/// <summary>
		/// Clear all nodes in the current dictionary
		/// </summary>
		public override void ClearAllNodes()
		{
		    for (int i = 0; i < _listOfNodes.Count; i++)
		    {
		        _listOfNodes[i].Cancel();
		        _listOfNodes.RemoveAt( i );
                i--;
		    }
		}
		/// <summary>
		/// Clear all nodes in the current dictionary with the passed tag
		/// </summary>
		public override void ClearNodesWithTag(string p_tag)
		{
		    for (int i = 0; i < _listOfNodes.Count; i++)
		    {
                if (_listOfNodes[i].tag == p_tag)
                {
                    _listOfNodes[i].Cancel( );
                    _listOfNodes.RemoveAt( i );
                    i--;
                }
		    }
		}
		/// <summary>
		/// Clear all nodes in the current dictionary except for the nodes with the passed tag
		/// </summary>
		public override void ClearNodesExceptForTag(string p_tag)
		{
		    for (int i = 0; i < _listOfNodes.Count; i++)
		    {
		        if (_listOfNodes[i].tag != p_tag)
		        {
		            _listOfNodes[i].Cancel( );
		            _listOfNodes.RemoveAt( i );
		            i--;
                }
		    }
        }
		#endregion
	}
}