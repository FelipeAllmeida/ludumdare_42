using UnityEngine;
using System.Collections;

namespace VoxInternal
{
	public abstract class PhysicsDetector : MonoBehaviour
	{
		public enum ColliderType
		{
			SPHERE,
			CUBE_3D,
			CIRCLE_2D,
			SQUARE_2D
		}

		[Header ("Debug Graphics Settings")]
		[SerializeField]
		protected bool _debugGraphics = true;

		[SerializeField]
		protected bool _debugVisuallyOnlyListeningEvents = true;

		[SerializeField]
		protected Color triggerStandardColor = new Color(1f,0.0f, 0f, 0.3f);

		[SerializeField]
		protected Color selectedOrTriggeredColor = new Color(1f,0.8f, 0f, 0.9f);

		protected ColliderType _triggerShape;
		protected BoxCollider _boxCollider3d;
		protected BoxCollider2D _boxCollider2d;
		protected CircleCollider2D _circleCollider2d;
		protected SphereCollider _sphereCollider;
		protected bool _isTriggered = false;

		protected bool CheckColliderType()
		{
			_boxCollider3d = GetComponent<BoxCollider> ();

			if (_boxCollider3d != null) {
				_triggerShape = ColliderType.CUBE_3D;
				return true;
			}

			_circleCollider2d = GetComponent<CircleCollider2D> ();

			if (_circleCollider2d != null) {
				_triggerShape = ColliderType.CIRCLE_2D;
				return true;
			}

			_boxCollider2d = GetComponent<BoxCollider2D> ();

			if (_boxCollider2d != null) {
				_triggerShape = ColliderType.SQUARE_2D;
				return true;
			}

			_sphereCollider = GetComponent<SphereCollider> ();

			if (_sphereCollider != null) {
				_triggerShape = ColliderType.SPHERE;
				return true;
			}

			return false;
		}

		// Use this for initialization
		protected void OnDrawGizmos ()
		{
			if (_debugGraphics == false)
				return;

			if (CheckColliderType () == false)
				return;

			Gizmos.color = triggerStandardColor;

			if(_isTriggered)
			{
				Gizmos.color = selectedOrTriggeredColor;
			}

			DrawShape ();
		}

		protected void OnDrawGizmosSelected()
		{
			if (_debugGraphics == false)
				return;

			if (CheckColliderType () == false)
				return;

			Gizmos.color = selectedOrTriggeredColor;

			DrawShape ();
		}

		protected void DrawShape()
		{

			Matrix4x4 __rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Gizmos.matrix = __rotationMatrix;

			switch (_triggerShape) 
			{
			case ColliderType.CUBE_3D:
				Gizmos.DrawCube (_boxCollider3d.center, new Vector3(_boxCollider3d.size.x, _boxCollider3d.size.y, _boxCollider3d.size.z));
				break;
			case ColliderType.CIRCLE_2D:
				Gizmos.DrawSphere (new Vector3(_circleCollider2d.offset.x, _circleCollider2d.offset.y,0), _circleCollider2d.radius * transform.localScale.magnitude/2);
				break;
			case ColliderType.SQUARE_2D:
				Gizmos.DrawCube (new Vector3(_boxCollider2d.offset.x, _boxCollider2d.offset.y,0),  new Vector3(_boxCollider2d.size.x, _boxCollider2d.size.y * transform.localScale.y, 1));
				break;
			case ColliderType.SPHERE:
				Gizmos.DrawSphere (_sphereCollider.center, _sphereCollider.radius);
				break;
			}
		}
	}
}