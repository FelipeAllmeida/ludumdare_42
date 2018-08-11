using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MapObject
{
    public enum WallType
    {
        WALL,
        DOOR
    }

    [SerializeField] private WallType _type;
    public WallType Type { get { return _type; } }

    public bool IsHorizontal { get { return transform.localScale.x > transform.localScale.z; } }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
