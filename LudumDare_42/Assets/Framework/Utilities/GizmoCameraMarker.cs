using UnityEngine;
using System.Collections;

public class GizmoCameraMarker : GizmoMarker 
{
	protected override void Draw ()
	{
		base.Draw ();
		var __length = Camera.main.fieldOfView / 100;
		var __lineLength = 0.55f;
		
		Vector3 directionTL = transform.TransformDirection(new Vector3(-__length, -__length, 1.0f) * __lineLength) + transform.position;
		Vector3 directionTR = transform.TransformDirection(new Vector3(__length, -__length, 1.0f) * __lineLength) + transform.position;
		Vector3 directionBL = transform.TransformDirection(new Vector3(-__length, __length, 1.0f) * __lineLength) + transform.position;
		Vector3 directionBR = transform.TransformDirection(new Vector3(__length, __length, 1.0f) * __lineLength) + transform.position;
		
		Gizmos.DrawLine(transform.position, directionTL);
		Gizmos.DrawLine(transform.position, directionTR);
		Gizmos.DrawLine(transform.position, directionBL);
		Gizmos.DrawLine(transform.position, directionBR);

		Gizmos.DrawLine(directionBL, directionTL);
		Gizmos.DrawLine(directionTL, directionTR);
		Gizmos.DrawLine(directionTR, directionBR);
		Gizmos.DrawLine(directionBR, directionBL);
	}
}
