using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentData
{
    public EnvironmentType currentEnvironment;

    private EnvironmentData() { }

    public EnvironmentData(EnvironmentType p_currentEnvironment)
    {
        currentEnvironment = p_currentEnvironment;
    }
}
