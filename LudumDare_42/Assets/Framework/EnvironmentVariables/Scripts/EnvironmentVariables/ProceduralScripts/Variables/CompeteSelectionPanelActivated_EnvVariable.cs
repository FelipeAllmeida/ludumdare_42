using System;
public static partial class EnvironmentVariables
{
	private static readonly bool Production_CompeteSelectionPanelActivated = false;
	private static readonly bool Test_CompeteSelectionPanelActivated = true;
	private static readonly bool Development_CompeteSelectionPanelActivated = true;
	private static readonly bool Custom_CompeteSelectionPanelActivated = true;
	private static readonly bool RC_CompeteSelectionPanelActivated = false;
	public static bool CompeteSelectionPanelActivated
	{
		get
		{
			switch(currentEnvironment)
			{
				case EnvironmentType.Production: 
					return Production_CompeteSelectionPanelActivated;

				case EnvironmentType.Test: 
					return Test_CompeteSelectionPanelActivated;

				case EnvironmentType.Development: 
					return Development_CompeteSelectionPanelActivated;

				case EnvironmentType.Custom: 
					return Custom_CompeteSelectionPanelActivated;

				case EnvironmentType.RC: 
					return RC_CompeteSelectionPanelActivated;

				default: 
					return false;
			}
		}
	}
}