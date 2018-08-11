/*
 * Vers.: 1.0
 * Made by Andre Schaan (Unity 3D programmer)
 * This classe is part of Vox's Framework
*/

using UnityEngine;
using System.Collections;


/// <summary>
/// UV scroller UV data.
/// </summary>
/// <remarks>
/// Available property names:<BR>
/// -> "_MainTex"<BR>
/// -> "_BumpMap"<BR>
/// -> "_Cube"<BR>
/// </remarks>
[System.Serializable]
public class UVScrollerUVData
{
    public string propertyName = "_MainTex";
    public Vector2 direction;

    private Vector2 _currentOffset;
    public Vector2 currentOffset
    {
        get { return _currentOffset; }
        set { _currentOffset = value; }
    }
}

 


/// <summary>
/// UV scroller.
/// </summary>
/// <remarks>
/// Just drag and drop this script into a GameObject with a material.<BR>
/// You can set up an array of UVScrollers that will take one property of the material and move it with a given vector.
/// </remarks>
public class UVScroller : MonoBehaviour
{
    public UVScrollerUVData[] uv;  

    private Material _thisMaterial;

	void Start () 
    {
        _thisMaterial = transform.GetComponent<Renderer>().material;

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i].currentOffset += _thisMaterial.GetTextureOffset(uv[i].propertyName);
        }
	}
	
	void Update () 
    {
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i].currentOffset += uv[i].direction * Time.deltaTime;
            _thisMaterial.SetTextureOffset(uv[i].propertyName, uv[i].currentOffset);
        }
	}
}
