using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxInternal.Editor
{
	/// <summary>
	/// Options to define rendering layer of objects.
	/// </summary>
	public enum RenderTag
	{
		RESET_TO_DEFAULT = -1,
		HIGHEST_PRIORITY = 0,
		BACKGROUND = 1000,
		GEOMETRY = 2000,
		TRANSPARENT = 3000,
		OVERLAY = 4000,
	}

	/// <summary>
	/// Editor for materials and allowing them to select rendering order.
	/// </summary>
	public class VoxMaterialEditor : MaterialEditor  {
		
		public override void OnInspectorGUI ()
		{	
			// if we are not visible... return
			if (!isVisible)
				return;

			// get the current keywords from the material
			Material targetMat = target as Material;

			GUIContent title = new GUIContent("Vox Custom Material Editor", "Joguinhos");

			EditorGUILayout.LabelField(title);
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUI.BeginChangeCheck();

			RenderSortingControl (targetMat);

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty (targetMat);
			}

			// render the default inspector
			base.OnInspectorGUI ();

		}

		private void RenderSortingControl(Material material)
		{
			GUIContent content = new GUIContent("Render Queue Sorting: " + material.renderQueue.ToString() , "Defines the order of the geometry to be rendered by the engine");

			EditorGUILayout.LabelField(content);
			EditorGUILayout.Space ();

			RenderTag __tag = RenderTag.BACKGROUND;

			if (material.renderQueue >= 0 && material.renderQueue < 1000) {
				__tag = RenderTag.HIGHEST_PRIORITY;
			} else if (material.renderQueue >= 1000 && material.renderQueue < 2000) {
				__tag = RenderTag.BACKGROUND;
			} else if (material.renderQueue >= 2000 && material.renderQueue < 3000) {
				__tag = RenderTag.GEOMETRY;
			} else if (material.renderQueue >= 3000 && material.renderQueue < 4000) {
				__tag = RenderTag.TRANSPARENT;
			} else if (material.renderQueue >= 4000) {
				__tag = RenderTag.OVERLAY;
			}

			int __orderInLayer = material.renderQueue - (int)__tag;

			GUIContent content2 = new GUIContent("    Layer", "Rendering Layer. Background render first and overlay last. Represents a range of renderqueue values");

			__tag = (RenderTag)EditorGUILayout.EnumPopup (content2, __tag);

			if (__tag != RenderTag.RESET_TO_DEFAULT) {
				GUIContent content3 = new GUIContent ("    Order in Layer", "Value must be positive to work. Ranges from 0 to 999. Use -1 to use default shader renderQueue order");
				
				if (__orderInLayer < 0 || __orderInLayer > 999) {
					__orderInLayer = Mathf.Clamp (__orderInLayer, 0, 999);
				}
				__orderInLayer = EditorGUILayout.IntField (content3, __orderInLayer);

				material.renderQueue = (int)__tag + __orderInLayer;
			} 
			else 
			{
				material.renderQueue = -1;
			}
			EditorGUILayout.Space ();
		}
	}
}