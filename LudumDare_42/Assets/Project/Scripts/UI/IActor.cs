using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public enum ObjectAction
{
    [Description("Interact")] Interact,
    [Description("Cancel")] Cancel,
}

public interface IActor
{
    List<ObjectAction> ActionsList { get; }
}
