using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxInternal
{
	/// <summary>
	/// The center of the Tween, Sound, Thread and Timer modules. It updates and manage all these modules.
	/// </summary>
	public class ModuleCore
	{
		#region Instance Initialization
		/// <summary>
		/// Create automatically the unique instance of this class.
		/// </summary>
		static private ModuleCore _instance;
		static public ModuleCore instance
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

		/// <summary>
		/// Handles the logic required in the inicialization of the instance.
		/// </summary>
		static private ModuleCore InstanceInitialize()
		{
			ModuleCore __instance = new ModuleCore ();

			GameCore.instance.AddModuleCore (__instance);

			return __instance;
		}

		#endregion

		#region Private Data
		private List<Module>_moduleList = new List<Module>();

		#endregion

		#region Module Control
		/// <summary>
		/// Function to be called from the module to send itself to this core.
		/// </summary>
		public void AddModule(Module p_module)
		{
			_moduleList.Add (p_module);
		}

		/// <summary>
		/// Function to be called from the module to remove itself to this core.
		/// </summary>
		public void RemoveModule(Module p_module)
		{
			_moduleList.Remove (p_module);
		}

		/// <summary>
		/// Function that makes all added modules to clear its nodes.
		/// </summary>
		public void ClearAllNodes()
		{
			for (int i = 0; i < _moduleList.Count; i++) 
			{
				_moduleList [i].ClearAllNodes();
			}
		}

		/// <summary>
		/// Function that makes all added modules to clear the nodes with the passed tag.
		/// </summary>
		public void ClearModulesWithTag(string p_tag)
		{
			for (int i = 0; i < _moduleList.Count; i++) 
			{
				_moduleList [i].ClearNodesWithTag(p_tag);
			}
		}

		/// <summary>
		/// Function that makes all added modules to clear the nodes except for the ones with the passed tag.
		/// </summary>
		public void ClearModulesExceptTag(string p_tag)
		{
			for (int i = 0; i < _moduleList.Count; i++) 
			{
				_moduleList [i].ClearNodesExceptForTag (p_tag);
			}
		}
		#endregion

		#region Main Messages
		/// <summary>
		/// Function that updates the modules of the game. Which in their turn will update their nodes.
		/// </summary>
		public void ModulesUpdate()
		{
			for (int i = 0; i < _moduleList.Count; i++) 
			{
				_moduleList [i].ModuleUpdate ();
			}
		}

		/// <summary>
		/// Function that physics updates the modules of the game. Which in their turn will update their nodes.
		/// </summary>
		public void ModulesFixedUpdate()
		{
			for (int i = 0; i < _moduleList.Count; i++) 
			{
				_moduleList [i].ModuleFixedUpdate ();
			}
		}

		/// <summary>
		/// Function that late updates the modules of the game. Which in their turn will update their nodes.
		/// </summary>
		public void ModulesLateUpdate()
		{
			for (int i = 0; i < _moduleList.Count; i++) 
			{
				_moduleList [i].ModuleLateUpdate ();
			}
		}

		/// <summary>
		/// Function that drawn any GUI called from the modules of the game.
		/// </summary>
		public void ModulesOnGUI()
		{
			for (int i = 0; i < _moduleList.Count; i++) 
			{
				_moduleList [i].ModuleOnGUI ();
			}
		}
		#endregion
	}
}
