#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System;
using System.Collections;

namespace Vox.Utilities
{
	/// <summary>
	/// Data of UVScroller.
	/// </summary>
	/// <remarks>
	/// Data of UVScroller.
	/// </remarks>
	[Serializable]
	public class UVScrollerUVData
	{
		/// <summary>
		/// Name of material property to be modified.
		/// </summary>
		/// <remarks>
		/// Name of material property to be modified.
		/// </remarks>
	    public string propertyName = "_MainTex";
		/// <summary>
		/// MovingDirection which scroll will take.
		/// </summary>
		/// <remarks>
		/// MovingDirection which scroll will take.
		/// </remarks>
	    public Vector2 direction;
		/// <summary>
		/// Offset of the UV.
		/// </summary>
		/// <remarks>
		/// Offset of the UV.
		/// </remarks>
		public Vector2 currentOffset;
	}

	/// <summary>
	/// Class that scrolls UV of objects.
	/// </summary>
	/// <remarks>
	/// Class that scrolls UV of objects, being useful for effects such as faking water movement. 
	/// Start and update using unity functions.
	/// </remarks>
	public class UVScroller : MonoBehaviour
	{
		/// <summary>
		/// Vox.Utilites.UVScrollerUVData array.
		/// </summary>
		/// <remarks>
		/// Vox.Utilites.UVScrollerUVData array which is used to move a set of uvs.
		/// </remarks>
	    public UVScrollerUVData[] uv;  

		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

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

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion
	}
}
