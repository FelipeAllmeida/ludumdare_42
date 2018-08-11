using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Report
{
    public string user;
    public string id;
    public string title;
    public string description;
    public List<string> listScreenShots = new List<string>();
}
