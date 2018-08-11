#define SKIP_CODE_IN_DOCUMENTATION
using System;
using UnityEngine;
using VoxInternal;

namespace Vox
{
    /// <summary>
    /// Class to acess data and info created using Vox Editor Window.
    /// </summary>
    /// <remarks>
    /// Editor data Manager is the class used to acess data stored via the Vox Editor Window. You can acess the Editor Window via the Vox Tab in the top of the Unity editor.
    /// 
    /// <code>
    /// 
    /// public enum DifficultyModeType
    /// {
    /// 	EASY,
    /// 	NORMAL,
    /// 	HARD
    /// };
    /// 
    /// private DifficultyModeType _difficultyMode;
    /// 
    /// void SceneStart()
    /// {
    /// 	// Get the editor data of the difficulty mode and assing to _difficultyMode. The data was stored in the editor with its key as "DifficuldyMode"
    /// 
    /// 	Vox.EditorDataManager.GetField<DifficultyModeType>("DifficuldyMode", delegate(DifficultyModeType p_mode)
    /// 	{
    /// 		_difficultyMode = p_mode;
    /// 	});
    /// }
    /// </code>
    ///  
    /// </remarks> 
    public class EditorDataManager
    {
        private const string _fieldDataPath = "VoxEditorData/FieldDatas/";

        /// <summary>
        /// Get FieldData created in Vox Editor Window with the passed key.
        /// </summary>
        /// <remarks>
        /// Get a FieldData created in Vox Editor Window with the passed key.
        ///
        /// <code>
        /// 
        /// public enum DifficultyModeType
        /// {
        /// 	EASY,
        /// 	NORMAL,
        /// 	HARD
        /// };
        /// 
        /// private DifficultyModeType _difficultyMode;
        /// 
        /// void SceneStart()
        /// {
        /// 	// Get the editor data of the difficulty mode and assing to _difficultyMode. The data was stored in the editor with its key as "DifficuldyMode"
        /// 
        /// 	Vox.EditorDataManager.GetField<DifficultyModeType>("DifficuldyMode", delegate(DifficultyModeType p_mode)
        /// 	{
        /// 		_difficultyMode = p_mode;
        /// 	});
        /// }
        /// </code>
        /// 
        /// </remarks>
        public static void GetField<T>(string p_id, Action<T> p_onReceiveFieldData)
        {
            FieldData __fieldData = GetFieldData(p_id);

            if (__fieldData != null)
            {
                if (Application.isEditor && __fieldData.enabledInEditor == false)
                {
                    return;
                }
                else if (Application.isEditor == false && __fieldData.enabledInBuild == false)
                {
                    return;
                }

                switch (typeof(T).ToString())
                {
                    case "System.Boolean":
                        p_onReceiveFieldData((T) (object) __fieldData.valueBool);
                        break;

                    case "UnityEngine.Color":
                        p_onReceiveFieldData((T) (object) __fieldData.valueColor);
                        break;

                    case "System.Single":
                        p_onReceiveFieldData((T) (object) __fieldData.valueFloat);
                        break;

                    case "UnityEngine.Rect":
                        p_onReceiveFieldData((T) (object) __fieldData.valueRect);
                        break;

                    case "System.Int32":
                        p_onReceiveFieldData((T) (object) __fieldData.valueInt);
                        break;

                    case "System.String":
                        p_onReceiveFieldData((T) (object) __fieldData.valueString);
                        break;

                    case "UnityEngine.Vector2":
                        p_onReceiveFieldData((T) (object) __fieldData.valueVector2);
                        break;

                    case "UnityEngine.Vector3":
                        p_onReceiveFieldData((T) (object) __fieldData.valueVector3);
                        break;

                    case "UnityEngine.KeyCode":
                        p_onReceiveFieldData((T) (object) __fieldData.valueKeycode);
                        break;

                    case "UnityEngine.AnimationCurve":
                        p_onReceiveFieldData((T) (object) __fieldData.valueAnimationCurve);
                        break;

                    default:
                        p_onReceiveFieldData((T) (object) __fieldData.valueEnumSelected);
                        break;
                }
            }
        }

        public static void GetField<T>(string p_id, int p_debugIndex, Action<T> p_onReceiveFieldData)
        {
            if (p_debugIndex == -1)
            {
                GetField<T>(p_id, p_onReceiveFieldData);
                return;
            }

            FieldData __fieldData = GetFieldData(p_id);

            if (__fieldData != null)
            {
                if (Application.isEditor && __fieldData.enabledInEditor == false)
                {
                    return;
                }
                else if (Application.isEditor == false && __fieldData.enabledInBuild == false)
                {
                    return;
                }

                if (__fieldData.listFieldDataDebugValues.Count > p_debugIndex)
                {
                    switch (typeof(T).ToString())
                    {
                        case "System.Boolean":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueBool);
                            break;

                        case "UnityEngine.Color":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueColor);
                            break;

                        case "System.Single":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueFloat);
                            break;

                        case "UnityEngine.Rect":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueRect);
                            break;

                        case "System.Int32":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueInt);
                            break;

                        case "System.String":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueString);
                            break;

                        case "UnityEngine.Vector2":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueVector2);
                            break;

                        case "UnityEngine.Vector3":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueVector3);
                            break;

                        case "UnityEngine.KeyCode":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueKeycode);
                            break;

                        case "UnityEngine.AnimationCurve":
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueAnimationCurve);
                            break;

                        default:
                            p_onReceiveFieldData((T) (object) __fieldData.listFieldDataDebugValues[p_debugIndex].valueEnumSelected);
                            break;
                    }
                }
            }
        }

        //
        //		/// <summary>
        //		/// Check if FieldData is enabled before using it.
        //		/// </summary>
        //		/// <remarks>
        //		/// </remarks>
        //		static public bool IsFieldEnabled(string p_id)
        //		{
        //			FieldData __fieldData = GetFieldData (p_id);
        //
        //			if (__fieldData == null) 
        //			{
        //				return false;
        //			}
        //
        //			if(Application.isEditor == true)
        //			{	
        //				if(__fieldData.enabledInEditor == false)
        //					return false;
        //				else
        //					return true;
        //			}
        //			if(Application.isEditor == false)
        //			{
        //				if(__fieldData.enabledInBuild == false)
        //					return false;
        //				else
        //					return true;
        //			}
        //
        //			return false;
        //		}

        #region Private Internal Only
#if SKIP_CODE_IN_DOCUMENTATION
        /// @cond
#endif

        /// <summary>
        /// Private function to iterate through VoxEditorData list of fields and return FieldData.
        /// </summary>
        /// <remarks>
        /// </remarks> 
        private static FieldData GetFieldData(string p_id)
        {
            TextAsset __fieldDataJson = Resources.Load<TextAsset>(_fieldDataPath + p_id) as TextAsset;

            if (__fieldDataJson == null)
            {
                Debug.LogWarning("Vox Editor: id " + p_id + " don't exist");

                return null;
            }

            FieldData __fieldData = JsonUtility.FromJson<FieldData>(__fieldDataJson.text) as FieldData;

            if (__fieldData == null)
            {
                Debug.LogWarning("Vox Editor: id " + p_id + " json not valid");

                return null;
            }

            return __fieldData;
        }

        /// <summary>
        /// Check if FPSDebugger is be enabled.
        /// </summary>
        /// <remarks>
        /// </remarks> 
        public static FieldDataEnabledState isFPSInBuildEnabled()
        {
            TextAsset __fieldDataEnabledStateJson = Resources.Load<TextAsset>("VoxEditorData/FPSConfig") as TextAsset;

            FieldDataEnabledState __fpsEnabledState = (FieldDataEnabledState) JsonUtility.FromJson<FieldDataEnabledState>(__fieldDataEnabledStateJson.text);

            return __fpsEnabledState;
        }

#if SKIP_CODE_IN_DOCUMENTATION

        /// @endcond
#endif
        #endregion
    }
}