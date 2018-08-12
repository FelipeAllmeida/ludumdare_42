using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class UIUtils
{
    public static string GetDescription(this Enum p_value)
    {
        DescriptionAttribute __description = p_value.GetType().GetField(p_value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute)).First() as DescriptionAttribute;
        return __description != null ? __description.Description : string.Empty;
    }

}
