using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSimulatorUIObject : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        InputSimulatorCore.AddUICanvas(this.gameObject);
    }
}
