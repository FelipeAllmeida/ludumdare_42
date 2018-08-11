//This code was auto generated, please do not edit it.

using UnityEngine;
public enum EnvironmentType
{
	Production,
	Test,
	Development,
	Custom,
	RC
}

public static partial class EnvironmentVariables
{
	[IgnoreEnvironmentVariable] private static string _environmentDataPath = "VoxEnvironmentVariablesData/CurrentEnvironment";
	[IgnoreEnvironmentVariable] private static bool IsEnvironmentLoaded = false;
	[IgnoreEnvironmentVariable] private static EnvironmentType _currentEnvironment;
	public static EnvironmentType currentEnvironment
{
		get
		{
#if Production_CloudEnvironment
			return EnvironmentType.Production;
#elif Development_CloudEnvironment
			return EnvironmentType.Development;
#elif Test_CloudEnvironment
			return EnvironmentType.Test;
#elif Custom_CloudEnvironment
			return EnvironmentType.Custom;
#elif RC_CloudEnvironment
			return EnvironmentType.RC;
#else
			if(IsEnvironmentLoaded == false)
			{
				IsEnvironmentLoaded = true;
				TextAsset __currentEnvironmentTextAsset = Resources.Load<TextAsset>(_environmentDataPath) as TextAsset;
				if (__currentEnvironmentTextAsset == null)
				{
					Debug.Log("CurrentEnvironmentTextAsset is null");
					return EnvironmentType.Development;
				}
				EnvironmentData __environmentData = JsonUtility.FromJson<EnvironmentData>(__currentEnvironmentTextAsset.text);
				 Debug.Log("Load Environment: " + __environmentData.currentEnvironment);
				_currentEnvironment = __environmentData.currentEnvironment;
			}
			return _currentEnvironment;
#endif
		}
	}
	public static void ForceSetCurrentEnvironment(EnvironmentType p_environmentType)
	{
		IsEnvironmentLoaded = true;
		_currentEnvironment = p_environmentType;
	}

}