using UnityEngine;
using System.Collections;


/// <summary>
/// Gizmo marker.
/// </summary>
/// <remarks>
/// This is a simple class, just drag and drop this script in a object that you want to mark with a sphere.<BR>
/// You can set a radius, offset and a color for the sphere.<BR>
/// Gizmos are not showed in builded versions of the game, so you can't see it in your iphone, android or runtime versions in windows, mac os or linux.<BR>
/// </remarks>
public class GizmoMarker : MonoBehaviour 
{
	private static bool _enabled = true;
	
	
	public float radius = 0.15f;
	public Color color = Color.white;
	public bool onSelectionOnly = false;
	public Vector3 offset;
	
	void OnDrawGizmosSelected()
	{	
		if(onSelectionOnly && _enabled)
		{
			Draw();
		}
	}
	
	void OnDrawGizmos()
	{	
		if(!onSelectionOnly && _enabled)
		{
			Draw();
		}
	}
	
	protected virtual void Draw()
	{
		//Define color
		Gizmos.color = color;
		Gizmos.DrawSphere(transform.position + offset, radius);
	}
	
	//Static methods
	public static void EnableAll()
	{
		_enabled = true;
	}
	
	public static void DisableAll()
	{
		_enabled = false;
	}
	
}
