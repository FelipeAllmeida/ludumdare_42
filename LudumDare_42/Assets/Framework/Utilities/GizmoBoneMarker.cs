using UnityEngine;
using System.Collections;

public class GizmoBoneMarker : GizmoMarker 
{
	protected override void Draw ()
	{
		base.Draw ();
		if(transform.parent != null)
		{
			Gizmos.DrawLine(transform.position + offset, transform.parent.transform.position);
		}
	}
}
