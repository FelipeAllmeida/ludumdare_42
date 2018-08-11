using System;
public static partial class EnvironmentVariables
{
	private static readonly bool Production_VersionVerificationEnabled = true;
	private static readonly bool Test_VersionVerificationEnabled = false;
	private static readonly bool Development_VersionVerificationEnabled = false;
	private static readonly bool Custom_VersionVerificationEnabled = false;
	private static readonly bool RC_VersionVerificationEnabled = false;
	public static bool VersionVerificationEnabled
	{
		get
		{
			switch(currentEnvironment)
			{
				case EnvironmentType.Production: 
					return Production_VersionVerificationEnabled;

				case EnvironmentType.Test: 
					return Test_VersionVerificationEnabled;

				case EnvironmentType.Development: 
					return Development_VersionVerificationEnabled;

				case EnvironmentType.Custom: 
					return Custom_VersionVerificationEnabled;

				case EnvironmentType.RC: 
					return RC_VersionVerificationEnabled;

				default: 
					return false;
			}
		}
	}
}