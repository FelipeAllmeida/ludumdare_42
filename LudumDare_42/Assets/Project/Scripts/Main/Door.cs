using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Wall
{
    public enum State
    {
        OPEN,
        CLOSED
    }

    public State CurrentState { get; private set; }
}
