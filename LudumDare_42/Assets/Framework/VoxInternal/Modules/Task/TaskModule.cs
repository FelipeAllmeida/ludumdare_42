#if NETFX_CORE
using System.Collections.Generic;
using Vox;

namespace VoxInternal
{
    [System.Serializable]
    public class TaskModule : Module
    {
#region Public Static Data
        /// <summary>
        /// Create automatically the unique instance of this class.
        /// </summary>
        static private TaskModule _instance;
        static public TaskModule instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = InstanceInitialize();
                }

                return _instance;
            }
        }
#endregion

#region Private Get-Only Data
        private List<TaskNode> _listOfNodes = new List<TaskNode>();

        public List<TaskNode> listOfNodes
        {
            get { return _listOfNodes; }
        }
#endregion

#region Singleton Instance Initialization
        /// <summary>
        /// Handles the logic required in the inicialization of the instance.
        /// </summary>
        static private TaskModule InstanceInitialize()
        {
            TaskModule __instance = new TaskModule();

            ModuleCore.instance.AddModule(__instance);

            return __instance;
        }
#endregion

#region Update Nodes
        public override void ModuleUpdate()
        {
            for (int i = 0; i < _listOfNodes.Count; i++)
            {
                if (_listOfNodes[i].ShouldNodeBeCleaned())
                {
                    _listOfNodes[i] = null;
                    _listOfNodes.RemoveAt(i);
                    i--;
                    continue;
                }
                _listOfNodes[i].UpdateNode();
            }
        }
#endregion

#region Node Control
        /// <summary>
        /// Function to be called from a TweenNode to add itself to this module dictionary.
        /// </summary>
        public void AddNode(TaskNode p_node)
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
                _listOfNodes.RemoveAt(i);
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
                    _listOfNodes[i].Cancel();
                    _listOfNodes.RemoveAt(i);
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
                    _listOfNodes[i].Cancel();
                    _listOfNodes.RemoveAt(i);
                    i--;
                }
            }
        }
#endregion
    }
}
#endif