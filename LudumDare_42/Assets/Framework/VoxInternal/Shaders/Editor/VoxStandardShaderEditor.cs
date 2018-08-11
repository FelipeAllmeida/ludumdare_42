using UnityEditor;
using UnityEngine;

namespace VoxInternal.Editor
{
	/// <summary>
	/// Vox custom inspector of standard shader to allow controll of Rendering order.
	/// </summary>
	internal class VoxStandardShaderEditor : StandardShaderGUIBase
	{
		MaterialProperty blendMode = null;

		public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			base.OnGUI (materialEditor, properties);
			
			Material __targetMaterial = (Material)materialEditor.target;
			
			GetProporties(properties);
			
			EditorGUI.BeginChangeCheck();
			
			RenderSortingControl (__targetMaterial);
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty (__targetMaterial);
			}
		}
		
		private void GetProporties(MaterialProperty[] properties)
		{
			blendMode = (FindProperty ("_Mode", properties));
		}
		
		private void RenderSortingControl(Material material)
		{
			EditorGUILayout.Space ();

			GUIContent content = new GUIContent("Render Queue Sorting: " + material.renderQueue.ToString() , "Relates to Rendering Mode");
			
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
			
			if (__tag != RenderTag.RESET_TO_DEFAULT) 
			{
				GUIContent content3 = new GUIContent ("    Order in Layer", "Value must be positive to work. Ranges from 0 to 999. Use -1 to use default shader renderQueue order");
				
				if (__orderInLayer < 0 || __orderInLayer > 999) {
					__orderInLayer = Mathf.Clamp (__orderInLayer, 0, 999);
				}
				__orderInLayer = EditorGUILayout.IntField (content3, __orderInLayer);
				
				material.renderQueue = (int)__tag + __orderInLayer;
			} 
			else 
			{
				BlendMode mode = (BlendMode)blendMode.floatValue;

				switch (mode)
				{
					case BlendMode.Opaque:
						material.renderQueue = -1;
						break;
					case BlendMode.Cutout:
						material.renderQueue = 2450;
						break;
					case BlendMode.Fade:

						material.renderQueue = 3000;
						break;
					case BlendMode.Transparent:
						material.renderQueue = 3000;
						break;
				 	default:
					break;
				}
			}
			EditorGUILayout.Space ();
		}
	}
}