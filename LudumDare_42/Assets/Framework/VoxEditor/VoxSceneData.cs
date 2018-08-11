using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class SceneData
{
	public string name;
	public string path;
	public int index;
	public bool enabled;
	
	public void Initialize()
	{
		path = string.Empty;
		enabled = false;
		index = -1;
	}
}

[Serializable]
public class VoxSceneData : ScriptableObject
{
	public List<SceneData> listScenes = new List<SceneData>();
};
	
