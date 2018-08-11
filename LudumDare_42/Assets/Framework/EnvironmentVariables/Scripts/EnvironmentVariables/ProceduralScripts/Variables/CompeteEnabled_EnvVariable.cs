using System;
public static partial class EnvironmentVariables
{
	private static readonly bool Production_CompeteEnabled = true;
	private static readonly bool Test_CompeteEnabled = true;
	private static readonly bool Development_CompeteEnabled = true;
	private static readonly bool Custom_CompeteEnabled = true;
    private static readonly bool RC_CompeteEnabled = true;

    public static bool CompeteEnabled
	{
		get
		{
			switch(currentEnvironment)
			{
				case EnvironmentType.Production: 
					return Production_CompeteEnabled;

				case EnvironmentType.Test: 
					return Test_CompeteEnabled;

				case EnvironmentType.Development: 
					return Development_CompeteEnabled;

				case EnvironmentType.Custom: 
					return Custom_CompeteEnabled;

                case EnvironmentType.RC:
                    return RC_CompeteEnabled;

                default: 
					return false;
			}
		}
	}
}