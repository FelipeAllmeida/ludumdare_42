using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxInternal
{
    public enum FiedDataOptionType
    {
        F1,
        F2,
        F3,
        F4
    };

    public enum FieldEditState
    {
        RENAMING,
        SETTING_VALUES
    }

    /// <summary>
    /// Class used to store a data of any of the types in class.
    /// </summary>
    [Serializable]
    public class FieldData
    {
        public FieldEditState editState = FieldEditState.SETTING_VALUES;
        public bool isArchived = false;
        public bool isProduction = false;
        public string archivedBy = "";
        public bool enabledInEditor = true;
        public bool enabledInBuild = false;
        public bool isUnfolded = false;
        public int orderInList;
        public string id = "";
        public string temporaryNewID = "";
        public EditorTabType tabParent;
        public int customTabNumber;
        public FieldType type = FieldType.BOOL;
        public bool valueBool;
        public Color valueColor = Color.white;
        public float valueFloat;
        public int valueInt;
        public Rect valueRect;
        public string valueString;
        public Vector2 valueVector2;
        public Vector3 valueVector3;
        public KeyCode valueKeycode;
        public AnimationCurve valueAnimationCurve = new AnimationCurve();
        public string valueEnum;
        public int valueEnumSelected;
        public Type valueTypeEnum;
        public List<FieldDataContent> listFieldDataDebugValues = new List<FieldDataContent>();
    }

    [Serializable]
    public class FieldDataContent
    {
        public KeyCode debugCode;
        public bool valueBool;
        public Color valueColor = Color.white;
        public float valueFloat;
        public int valueInt;
        public Rect valueRect;
        public string valueString;
        public Vector2 valueVector2;
        public Vector3 valueVector3;
        public KeyCode valueKeycode;
        public AnimationCurve valueAnimationCurve = new AnimationCurve();
        public string valueEnum;
        public int valueEnumSelected;
        public Type valueTypeEnum;
    }

    [Serializable]
    public class FoldData
    {
        public List<FieldFoldedData> listOfFoldedData = new List<FieldFoldedData>();
    }

    [Serializable]
    public class FieldFoldedData
    {
        public string id;
        public bool isUnfolded;
    }

    /// <summary>
    /// Selectable states that defines when FieldData is active.
    /// </summary>
    /// <remarks>
    /// Can be used for other classes which follows same logic.
    /// </remarks>
    [Serializable]
    public enum FieldDataEnabledState
    {
        DISABLED,
        ENABLED_EDITOR,
        ENABLED_BUILD,
        ALWAYS_ENABLED,
    }

    /// <summary>
    /// Available variables tipes to be used in FielData.
    /// </summary>
    [Serializable]
    public enum FieldType
    {
        BOOL,
        COLOR,
        ENUM,
        FLOAT,
        INT,
        RECT,
        STRING,
        VECTOR_2,
        VECTOR_3,
        KEYCODE,
        ANIMATION_CURVE
    }

    /// <summary>
    /// Types of tabs used in editor.
    /// </summary>
    [Serializable]
    public enum EditorTabType
    {
        DEFAULT = 0,
        DEBUG = 1,
        DATABASE,
        GAME,
        LOGIN
    };
}