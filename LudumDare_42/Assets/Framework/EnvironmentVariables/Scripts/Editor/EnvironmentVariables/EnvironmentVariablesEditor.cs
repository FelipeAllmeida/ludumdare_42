using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class EnvironmentVariablesEditor : EditorWindow
{
    #region Enums

    public enum EnvironmentVariablesEditorState
    {
        ADDING_VARIABLE,
        ADDING_ENVIRONMENT,
        REMOVING_ENVIRONMENT,
        EDITING
    }

    public enum VariableType
    {
        STRING,
        INT,
        BOOL,
        FLOAT,
        ENUM
    }

    #endregion

    #region Class

    public class EnvironmentVariableData
    {
        public string variableName;
        public string id;
        public VariableType type;
        public Type enumType;
        public Dictionary<EnvironmentType, object> dictOfValuebyEnvironment = new Dictionary<EnvironmentType, object>();
        public bool wasModified;
        public bool isUnfolded = false;
    }

    #endregion

    #region Const Values

    private const string WindowTitle = "Variables";
    private const string VariablesPath = "/Framework/EnvironmentVariables/Scripts/EnvironmentVariables/ProceduralScripts/Variables/";
    private const string DefaultStringValue = "\"\"";
    private const string DefaultIntegerValue = "0";
    private const string DefaultFloatValue = "0f";
    private const string DefaultBooleanValue = "false";
    private const string DefaultEnumValue = "0";
    private const string VariablesNameAcceptedCharacters = @"^(([0-9A-Z])(?!\2))*$";
    private const string EnvironmentVariableMainCodePath = "/Framework/EnvironmentVariables/Scripts/EnvironmentVariables/ProceduralScripts/EnvironmentVariables.cs";

    private const string MainCodeTemplate = "/Framework/EnvironmentVariables/Scripts/Editor/EnvironmentVariables/MainCodeTemplate.txt";

    #endregion

    #region Environment Variables
    private string _newEnvironmentName;
    private int _toRemoveEnvironmentIndex;
    #endregion

    #region New TempVariable Values

    private string _newVariableName;
    private static VariableType _newVariabletype = new VariableType();
    private static string _enumTypeStr = string.Empty;
    private static Vector2 _scrollPosition;
    private GUIStyle _styleHtml = new GUIStyle();

    #endregion

    #region Private Data
    private static string _environmentDataPath = "Framework/Resources/VoxEnvironmentVariablesData/CurrentEnvironment.json";

    private static string _defaultEmptyValue;

    private static Dictionary<string, EnvironmentVariableData> dictOfEnvVariablesByID = new Dictionary<string, EnvironmentVariableData>();
    private static EnvironmentVariablesEditorState _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
    private static EnvironmentType _currentEnvironmentSelected = new EnvironmentType();

    #endregion

    [MenuItem("Vox/EnvironmentVariables")]
    private static void Initialize()
    {
        EnvironmentVariablesEditor __window = (EnvironmentVariablesEditor)EditorWindow.GetWindow(typeof(EnvironmentVariablesEditor));
        __window.titleContent.text = WindowTitle;
        __window.titleContent.image = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Framework/Resources/VoxEditorData/VoxLogo32.png", typeof(Texture));

        __window.Show();
    }

    #region Enabling and Focusing

    private void OnEnable()
    {
        _styleHtml.richText = true;
        
        _currentEnvironmentSelected = (EnvironmentType)EditorPrefs.GetInt("CurrenEditorEnvironment", 0);

        LoadData();
    }

    private void LoadData()
    {
        Type __classType = typeof(EnvironmentVariables);
        FieldInfo[] __fields = __classType.GetFields(BindingFlags.NonPublic | BindingFlags.Static);

        foreach (FieldInfo field in __fields)
        {
            if (Attribute.IsDefined(field, typeof(IgnoreEnvironmentVariableAttribute)))
                continue;

            string __id = field.Name.Split('_')[1];

            if (dictOfEnvVariablesByID.ContainsKey(__id) == false)
            {
                EnvironmentVariableData __variable = new EnvironmentVariableData();
                __variable.id = field.Name.Split('_')[1];
                //important to get the real name on revert
                __variable.variableName = __variable.id;

                __variable.dictOfValuebyEnvironment = new Dictionary<EnvironmentType, object>();

                VariableType __variableTypeEnum = GetVariableEnumType(field.FieldType);
                __variable.type = __variableTypeEnum;
                if (__variableTypeEnum == VariableType.ENUM)
                {
                    __variable.enumType = Type.GetType((field.FieldType.Name + ",Assembly-CSharp").Replace(".", "+"));
                }

                foreach (EnvironmentType __environmentType in Enum.GetValues(typeof(EnvironmentType)))
                {
                    if (__variable.type == VariableType.ENUM)
                        __variable.dictOfValuebyEnvironment.Add(__environmentType, 0);
                    else
                        __variable.dictOfValuebyEnvironment.Add(__environmentType, GetDefaultValueForType(__variableTypeEnum));
                }

                dictOfEnvVariablesByID.Add(__variable.variableName, __variable);
            }

            string __environmentTypeName = field.Name.Split('_')[0];

            EnvironmentType __environmentTypeEnum = (EnvironmentType)Enum.Parse(typeof(EnvironmentType), __environmentTypeName);

            dictOfEnvVariablesByID[__id].dictOfValuebyEnvironment[__environmentTypeEnum] = field.GetValue((null));
        }
    }

    #endregion

    #region Disabling and Losing Focus

    private void OnDisable()
    {
        HandleExitingEditor();
    }

    private void OnLostFocus()
    {
        HandleExitingEditor();
    }

    private void HandleExitingEditor()
    {
        /* if(HasModifications())
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
            #endif
           /* if(EditorUtility.DisplayDialog("Forgot Data!", "You did not saved your data, do you want to save it now?", "Yes", "No"))
            {
                SaveAllVariables();
                UnityEditor.EditorApplication.UnlockReloadAssemblies();
            }
            else
            {
                UnityEditor.EditorApplication.UnlockReloadAssemblies();
            }
        }
        else
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
            #endif
        }*/
    }

    #endregion

    #region Draw

    private void OnGUI()
    {
        switch (_environmentEditorState)
        {
            case EnvironmentVariablesEditorState.ADDING_VARIABLE:
                DrawNewVariableCreationWindow();
                break;
            case EnvironmentVariablesEditorState.ADDING_ENVIRONMENT:
                DrawNewEnvironmentWindow();
                break;
            case EnvironmentVariablesEditorState.REMOVING_ENVIRONMENT:
                DrawRemoveEnvironmentWindow();
                break;
            case EnvironmentVariablesEditorState.EDITING:
                DrawEditingWindow();
                break;
            default:
                break;
        }
    }

    private void DrawNewEnvironmentWindow()
    {
        GUILayout.Label("Create New Environment", EditorStyles.boldLabel);
       
        _newEnvironmentName = EditorGUILayout.TextField("Name", _newEnvironmentName);

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Create"))
            {
                if (!string.IsNullOrEmpty(_newEnvironmentName))
                {
                    AddEnvironmentToMainEnvironmentCode(_newEnvironmentName);
                    SaveAllVariables();
                    _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Warning", "New Environment variable is null or empty.", "Ok"))
                    {
                        _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
                    }
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                _newEnvironmentName = string.Empty;
                _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
            }
        }
        GUILayout.EndHorizontal();
    }

    private void DrawRemoveEnvironmentWindow()
    {
        EnvironmentType[] __environmentTypeValues = (EnvironmentType[])Enum.GetValues(typeof(EnvironmentType));

        List<EnvironmentType> __listEnviromnentTypes = new List<EnvironmentType>(__environmentTypeValues);
        __listEnviromnentTypes.Remove(EnvironmentType.Production);
        __listEnviromnentTypes.Remove(EnvironmentType.Development);
        __listEnviromnentTypes.Remove(EnvironmentType.Test);

        if (__listEnviromnentTypes.Count > 0)
        {
            string[] __enumOptions = new string[__listEnviromnentTypes.Count];
            int[] __enumOptionsValues = new int[__listEnviromnentTypes.Count];

            for (int i = 0; i < __listEnviromnentTypes.Count; i ++)
            {
                if (_toRemoveEnvironmentIndex == -1)
                    _toRemoveEnvironmentIndex = (int)__listEnviromnentTypes[i];
                __enumOptions[i] = __listEnviromnentTypes[i].ToString();
                __enumOptionsValues[i] = (int)__listEnviromnentTypes[i];
            }

            _toRemoveEnvironmentIndex = EditorGUILayout.IntPopup("Build Target", _toRemoveEnvironmentIndex, __enumOptions, __enumOptionsValues);

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Remove"))
                {
                    RemoveEnvironmentToMainEnvironmentCode((EnvironmentType)_toRemoveEnvironmentIndex);
                    _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
                    _toRemoveEnvironmentIndex = -1;
                    LoadData();
                }

                if (GUILayout.Button("Cancel"))
                {
                    _toRemoveEnvironmentIndex = -1;
                    _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
                }
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            if (EditorUtility.DisplayDialog("Warning", "You must have at least one custom enviroment to remove.", "Ok"))
            {
                _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
            }
        }
    }

    private void DrawNewVariableCreationWindow()
    {
        GUILayout.Label("Create New Variable", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        _newVariableName = EditorGUILayout.TextField("Name", _newVariableName);

        if (EditorGUI.EndChangeCheck())
        {
            Regex rgx = new Regex(VariablesNameAcceptedCharacters, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(_newVariableName);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    _newVariableName = match.Value;
                }
            }
        }

        EditorGUI.BeginChangeCheck();

        _newVariabletype = (VariableType)EditorGUILayout.EnumPopup("Type", _newVariabletype);
        bool __canCreateIfEnumerator = false;
        if (_newVariabletype == VariableType.ENUM)
        {
            _enumTypeStr = EditorGUILayout.TextField("Enumerator Type", _enumTypeStr);
            string __enumTypeStrConvertedToAssembly = (_enumTypeStr + ",Assembly-CSharp").Replace(".", "+");

            if (Type.GetType(__enumTypeStrConvertedToAssembly) != null)
            {
                __canCreateIfEnumerator = true;
                GUILayout.Label("It is a valid enumerator.");
            }
            else
            {
                GUILayout.Label("Not a valid enumerator.");
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            FindDefaultValueForType(_newVariabletype);
        }

        GUILayout.BeginHorizontal();
        {
            GUI.color = Color.white;

            if (string.IsNullOrEmpty(_newVariableName) == false)
            {
                GUI.color = Color.green;
            }

            if (GUILayout.Button("Create"))
            {
                Action __createCallback = () =>
                {
                    if (!string.IsNullOrEmpty(_newVariableName))
                    {
                        EnvironmentVariableData newVar = new EnvironmentVariableData();
                        _newVariableName = _newVariableName.Substring(0, 1).ToUpper() + _newVariableName.Remove(0, 1);
                        newVar.variableName = _newVariableName;
                        newVar.id = _newVariableName;
                        newVar.type = _newVariabletype;
                        if (_newVariabletype == VariableType.ENUM)
                        {
                            string __enumTypeStrConvertedToAssembly = (_enumTypeStr + ",Assembly-CSharp").Replace(".", "+");
                            Type __enumType = Type.GetType(__enumTypeStrConvertedToAssembly);
                            if (__enumType != null)
                            {
                                newVar.enumType = __enumType;
                            }
                        }

                        newVar.dictOfValuebyEnvironment.Add(EnvironmentType.Test, _defaultEmptyValue);
                        SaveEnvironmentVariable(newVar);
                        _newVariableName = string.Empty;

                        _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
                    }
                };

                if (_newVariabletype == VariableType.ENUM)
                {
                    if (__canCreateIfEnumerator == true)
                    {
                        __createCallback();
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Warning", "The typed enum type does not exist", "Ok"))
                        {
                            _enumTypeStr = string.Empty;
                        }
                        
                    }
                }
                else
                {
                    __createCallback();
                }
            }

            GUI.color = Color.white;

            if (GUILayout.Button("Cancel"))
            {
                _environmentEditorState = EnvironmentVariablesEditorState.EDITING;
            }
        }
    }

    private void DrawEditingWindow()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinHeight(28));
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Local Environment: ");

                EditorGUI.BeginChangeCheck();
                {
                    _currentEnvironmentSelected = (EnvironmentType)EditorGUILayout.EnumPopup(_currentEnvironmentSelected);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt("CurrenEditorEnvironment", (int)_currentEnvironmentSelected);
                    SaveCurrentSelectedEnvironment(_currentEnvironmentSelected);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add Environment"))
                {
                    _environmentEditorState = EnvironmentVariablesEditorState.ADDING_ENVIRONMENT;
                }

                if (GUILayout.Button("Remove Environment"))
                {
                    _environmentEditorState = EnvironmentVariablesEditorState.REMOVING_ENVIRONMENT;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUI.color = Color.white;   

        DrawVariables();
    }

    private void DrawValue(EnvironmentVariableData p_fieldData)
    {
        foreach (EnvironmentType __environmentType in Enum.GetValues(typeof(EnvironmentType)))
        {
            switch (p_fieldData.type)
            {
                case VariableType.BOOL:

                    p_fieldData.dictOfValuebyEnvironment[__environmentType] = (object)EditorGUILayout.Toggle(__environmentType.ToString() + " value",
                        (bool)p_fieldData.dictOfValuebyEnvironment[__environmentType]);
                    break;

                case VariableType.INT:
                        p_fieldData.dictOfValuebyEnvironment[__environmentType] = (object)EditorGUILayout.IntField(__environmentType.ToString() + " value",
                            (int)p_fieldData.dictOfValuebyEnvironment[__environmentType]);
                    break;

                case VariableType.FLOAT:
                        p_fieldData.dictOfValuebyEnvironment[__environmentType] = (object)EditorGUILayout.FloatField(__environmentType.ToString() + " value",
                            (float)p_fieldData.dictOfValuebyEnvironment[__environmentType]);
                    break;

                case VariableType.STRING:
                        p_fieldData.dictOfValuebyEnvironment[__environmentType] = (object)EditorGUILayout.TextField(__environmentType.ToString() + " value",
                            (string)p_fieldData.dictOfValuebyEnvironment[__environmentType]);
                    break;

                case VariableType.ENUM:
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(__environmentType.ToString() + " value", GUILayout.Width(150f));
                            p_fieldData.dictOfValuebyEnvironment[__environmentType] = 
                                (object)EditorGUILayout.Popup((int)p_fieldData.dictOfValuebyEnvironment[__environmentType], Enum.GetNames(p_fieldData.enumType));
                        }
                        GUILayout.EndHorizontal();
                    break;
            }
        }
    }

    private void RevertChanges(EnvironmentVariableData p_data)
    {
        //important to force lost focus, otherwise the last field will stay with new value
        GUIUtility.keyboardControl = 0;
        Type __classType = typeof(EnvironmentVariables);

        foreach (EnvironmentType envType in Enum.GetValues(typeof(EnvironmentType)))
        {
            string __varName = envType + "_" + p_data.variableName;

            FieldInfo __field = __classType.GetField(__varName);

            //original id
            dictOfEnvVariablesByID[p_data.variableName].id = p_data.variableName;

            //original type
            VariableType __originalFieldType = GetVariableEnumType(__field.FieldType);
            dictOfEnvVariablesByID[p_data.variableName].type = __originalFieldType;

            //original dic env value
            dictOfEnvVariablesByID[p_data.variableName].dictOfValuebyEnvironment[envType] = __field.GetValue(null);

            //not edited
            dictOfEnvVariablesByID[p_data.variableName].wasModified = false;
        }
        AssetDatabase.Refresh();
    }

    private void DeleteVariable(EnvironmentVariableData p_data)
    {
        //find file in path and delete
        string __Path = Application.dataPath + VariablesPath + p_data.variableName + "_EnvVariable.cs";
        File.Delete(__Path);
        AssetDatabase.Refresh();
    }

    private void DrawVariables()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Variables", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();
                if (HasModifications())
                {
                   
                    GUI.color = NewColors.PaleGreen;

                    if (GUILayout.Button("Save all", GUILayout.Width(100)))
                    {
                        SaveAllVariables();
                    }
                }


                GUI.color = Color.white;   
                GUI.color = Color.green;

                // Color myColor = new Color();
                // ColorUtility.TryParseHtmlString ("#99ccff", out myColor);

                GUI.color = NewColors.PowderBlue;
                if (GUILayout.Button("Add Variable"))
                {
                    _environmentEditorState = EnvironmentVariablesEditorState.ADDING_VARIABLE;
                }
            }
            GUILayout.EndHorizontal();

            GUI.color = Color.white;

            if (dictOfEnvVariablesByID.Keys.Count == 0)
            {
                GUILayout.Label("No Variables To Display");
            }
            else
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

                foreach (string key in dictOfEnvVariablesByID.Keys)
                {
                    EnvironmentVariableData __variable = dictOfEnvVariablesByID[key];

                    GUI.backgroundColor = __variable.wasModified ? NewColors.FireBrick : Color.white;// new  Color(0.8f, 0.1f, 0.1f, 0.4f) : Color.white;

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        GUI.backgroundColor = Color.white;

                        GUILayout.BeginHorizontal();
                        {
                            __variable.isUnfolded = EditorGUILayout.Foldout(__variable.isUnfolded, string.Empty);
                            GUILayout.Space(-40);
                            GUILayout.Label("<b><size=12><color=#0066ff> " + __variable.type.ToString().ToLower() + ": </color></size></b>", _styleHtml);
                            GUILayout.Label("<b><size=12>" + __variable.id + "</size></b>", _styleHtml);

                            GUILayout.FlexibleSpace();


                            if (__variable.wasModified)
                            {
                                GUI.color = NewColors.SpringGreen;

                                if (GUILayout.Button("Save changes", GUILayout.Width(100)))
                                {
                                    if (__variable.wasModified)
                                        __variable.wasModified = false;
                                    
                                    SaveEnvironmentVariable(__variable, true);
                                }

                                GUI.color = NewColors.Orange;
                                if (GUILayout.Button("Revert changes", GUILayout.Width(100)))
                                {
                                    RevertChanges(__variable);
                                }
                                GUI.color = Color.white;
                            }


                            GUI.color = Color.red;

                            if (GUILayout.Button("X", GUILayout.Width(40)))
                            {
                                if (EditorUtility.DisplayDialog("Deleting variable!", "Do you want to delete it now?", "Yes", "No"))
                                {
                                    DeleteVariable(__variable);
                                }
                            }

                            GUI.color = Color.white;

                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(4);

                        if (__variable.isUnfolded)
                        {
                            EditorGUI.BeginChangeCheck();

                            GUI.color = Color.white;
                            DrawValue(__variable);

                            if (EditorGUI.EndChangeCheck())
                            {
                                __variable.wasModified = true;
                            }

                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
        }
        GUILayout.EndVertical();
    }

    #endregion

    #region Saving
    public static string SetAndSaveCurrentEnvironment(EnvironmentType p_currentEnvironment)
    {
        string __logArgs = "Try SetAndSaveCurrentEnvironment: " + p_currentEnvironment + "\n";

        try
        {
            StreamWriter __fieldDataWriter = File.CreateText(Application.dataPath + "/" + _environmentDataPath);
            __fieldDataWriter.WriteLine(JsonUtility.ToJson(new EnvironmentData(p_currentEnvironment), true));
            __fieldDataWriter.Close();
        }
        catch(Exception p_exception)
        {
            __logArgs += "Exception: " + p_exception + "\n";
        }

        return __logArgs;
    }

    private void SaveCurrentSelectedEnvironment(EnvironmentType p_currentEnvironmentSelected)
    {
        StreamWriter __fieldDataWriter = File.CreateText(Application.dataPath + "/" + _environmentDataPath);
        __fieldDataWriter.WriteLine(JsonUtility.ToJson(new EnvironmentData(p_currentEnvironmentSelected), true));
        __fieldDataWriter.Close();

        AssetDatabase.Refresh();

        Debug.Log("Created " + Application.dataPath + EnvironmentVariableMainCodePath);
    }

    private void AddEnvironmentToMainEnvironmentCode(string p_newEnvironmentType)
    {
        if (p_newEnvironmentType != string.Empty)
        {
            List<string> __listCustomEnviromentNames = GetListCustomEnvironmentNames();
            __listCustomEnviromentNames.Add(p_newEnvironmentType);
            SaveSupportedEnviromentsCode(__listCustomEnviromentNames);
            AssetDatabase.Refresh();
            LoadData();
        }
    }

    private void RemoveEnvironmentToMainEnvironmentCode(EnvironmentType p_environmentType)
    {
        if (p_environmentType != EnvironmentType.Development 
            && p_environmentType != EnvironmentType.Production 
            && p_environmentType != EnvironmentType.Test)
        {
            List<string> __listCustomEnvironmentNames = GetListCustomEnvironmentNames();
            __listCustomEnvironmentNames.Remove(p_environmentType.ToString());
            _currentEnvironmentSelected = EnvironmentType.Development;
            SaveSupportedEnviromentsCode(__listCustomEnvironmentNames);
        }
    }

    private void SaveSupportedEnviromentsCode(List<string> p_listCustomEnvironment)
    {      
        System.IO.File.WriteAllText(Application.dataPath + EnvironmentVariableMainCodePath, 
            GenerateSupportedEnvironmentsCode(p_listCustomEnvironment));

        Debug.Log("Created " + Application.dataPath + EnvironmentVariableMainCodePath);

        AssetDatabase.Refresh();
    }

    private List<string> GetListCustomEnvironmentNames()
    {
        EnvironmentType[] __environmentTypeValues = (EnvironmentType[])Enum.GetValues(typeof(EnvironmentType));
        List<string> __listCustomEnvironmentTypes = new List<string>();
        for (int i = 0; i < __environmentTypeValues.Length; i++)
        {
            if (__environmentTypeValues[i] != EnvironmentType.Production
                && __environmentTypeValues[i] != EnvironmentType.Development
                && __environmentTypeValues[i] != EnvironmentType.Test)
                __listCustomEnvironmentTypes.Add(__environmentTypeValues[i].ToString());
        }

        return __listCustomEnvironmentTypes;
    }

    public string GenerateSupportedEnvironmentsCode(List<string> p_listCustomEnvironmentTypes)
    {
        string __code = "//This code was auto generated, please do not edit it.\n\n";
        __code += "using UnityEngine;\n";
        __code += "public enum EnvironmentType\n";
        __code += "{\n";
        __code += "\tProduction,\n";
        __code += "\tTest,\n";

        if (p_listCustomEnvironmentTypes != null && p_listCustomEnvironmentTypes.Count > 0)
        {
            __code += "\tDevelopment,\n";
            for (int i = 0; i < p_listCustomEnvironmentTypes.Count; i++)
            {
                __code += "\t" + p_listCustomEnvironmentTypes[i];
                __code += (i < p_listCustomEnvironmentTypes.Count - 1) ? ",\n" : "\n";
            }
        }
        else
            __code += "\tDevelopment\n";

        __code += "}\n\n";

        __code += "public static partial class EnvironmentVariables\n";
        __code += "{\n";
        __code += "\t[IgnoreEnvironmentVariable] private static string _environmentDataPath = \"VoxEnvironmentVariablesData/CurrentEnvironment\";\n";
        __code += "\t[IgnoreEnvironmentVariable] private static bool IsEnvironmentLoaded = false;\n";
        __code += "\t[IgnoreEnvironmentVariable] private static EnvironmentType _currentEnvironment;\n";
        __code += "\tpublic static EnvironmentType currentEnvironment\n{\n";
        __code += "\t\tget\n\t\t{\n";
        __code += "#if Production_CloudEnvironment\n";
        __code += "\t\t\treturn EnvironmentType.Production;\n";
        __code += "#elif Development_CloudEnvironment\n";
        __code += "\t\t\treturn EnvironmentType.Development;\n";
        __code += "#elif Test_CloudEnvironment\n";
        __code += "\t\t\treturn EnvironmentType.Test;\n";

        if (p_listCustomEnvironmentTypes != null && p_listCustomEnvironmentTypes.Count > 0)
        {
            for (int i = 0; i < p_listCustomEnvironmentTypes.Count; i++)
            {
                __code += "#elif " + p_listCustomEnvironmentTypes[i] + "_CloudEnvironment\n";
                __code += "\t\t\treturn EnvironmentType."+ p_listCustomEnvironmentTypes[i] + ";\n";
            }
        }

        __code += "#else\n";

        __code += "\t\t\tif(IsEnvironmentLoaded == false)\n\t\t\t{\n";
        __code += "\t\t\t\tIsEnvironmentLoaded = true;\n";
        __code += "\t\t\t\tTextAsset __currentEnvironmentTextAsset = Resources.Load<TextAsset>(_environmentDataPath) as TextAsset;\n";
        __code += "\t\t\t\tif (__currentEnvironmentTextAsset == null)\n\t\t\t\t{\n";
        __code += "\t\t\t\t\tDebug.Log(\"CurrentEnvironmentTextAsset is null\");\n";
        __code += "\t\t\t\t\treturn EnvironmentType.Development;\n\t\t\t\t}\n";
        __code += "\t\t\t\tEnvironmentData __environmentData = JsonUtility.FromJson<EnvironmentData>(__currentEnvironmentTextAsset.text);\n";
        __code += "\t\t\t\t Debug.Log(\"Load Environment: \" + __environmentData.currentEnvironment);\n";
        __code += "\t\t\t\t_currentEnvironment = __environmentData.currentEnvironment;\n\t\t\t}\n";
        __code += "\t\t\treturn _currentEnvironment;\n";

        __code += "#endif\n\t\t}\n\t}\n";
        __code += "\tpublic static void ForceSetCurrentEnvironment(EnvironmentType p_environmentType)\n";
        __code += "\t{\n";
        __code += "\t\tIsEnvironmentLoaded = true;\n";
        __code += "\t\t_currentEnvironment = p_environmentType;\n";
        __code += "\t}\n";
        __code += "\n}";
        return __code;
    }

    private void SaveEnvironmentVariable(EnvironmentVariableData p_data, bool p_replaceValue = false)
    {
        string __type = "";

        bool __nameChanged = p_data.variableName != p_data.id;

        if (__nameChanged)
        {
            string __oldName = Application.dataPath + VariablesPath + p_data.variableName + "_EnvVariable.cs";
            string __newName = Application.dataPath + VariablesPath + p_data.id + "_EnvVariable.cs";

            File.Move(__oldName, __newName);
        }

        string __name = __nameChanged ? p_data.id : p_data.variableName;

        string __file = Application.dataPath + VariablesPath + __name + "_EnvVariable.cs";

        string __code = "using System;\n";
        __code += "public static partial class EnvironmentVariables\n{\n";

        string __getField = "";

        switch(p_data.type)
        {
            case VariableType.BOOL:
            case VariableType.FLOAT:
            case VariableType.INT:
            case VariableType.STRING:
                __type = p_data.type.ToString().ToLower();
                __getField = "\tpublic static " + __type + " " + p_data.id + "\n\t{\n\t\tget\n\t\t{\n\t\t\tswitch(currentEnvironment)\n\t\t\t{\n";
                break;
            case VariableType.ENUM:
                __type = p_data.type.ToString().ToLower();
                __getField = "\tpublic static " + p_data.enumType + " " + p_data.id + "\n\t{\n\t\tget\n\t\t{\n\t\t\tswitch(currentEnvironment)\n\t\t\t{\n";
                break;
        }

        foreach (EnvironmentType __environmentType in Enum.GetValues(typeof(EnvironmentType)))
        {
            FindDefaultValueForType(p_data.type);

            string __value = _defaultEmptyValue;

            if (p_replaceValue)
            {
                if (p_data.type == VariableType.STRING)
                {
                    __value = " \"" + p_data.dictOfValuebyEnvironment[__environmentType] + "\"";
                }
                else if (p_data.type == VariableType.BOOL)
                {
                    __value = p_data.dictOfValuebyEnvironment[__environmentType].ToString().ToLower();
                }
                else if (p_data.type == VariableType.ENUM)
                {
                    __value = p_data.dictOfValuebyEnvironment[__environmentType].ToString();
                }
                else
                {
                    __value = p_data.dictOfValuebyEnvironment[__environmentType].ToString();
                }
            }

            switch(p_data.type)
            {
                case VariableType.BOOL:
                case VariableType.FLOAT:
                case VariableType.INT:
                case VariableType.STRING:
                    __code += String.Format("\tprivate static readonly {0} {1} = {2};\n", __type, __environmentType + "_" + p_data.id, __value);
                    __getField += "\t\t\t\tcase EnvironmentType." + __environmentType + ": \n" + "\t\t\t\t\treturn " + __environmentType + "_" + p_data.id + ";\n\n";
                    break;
                case VariableType.ENUM:
                    string[] __enumValues = (string[])Enum.GetNames(p_data.enumType);
                    __code += String.Format("\tprivate static readonly {0} {1} = {2};\n", p_data.enumType, __environmentType + "_" + p_data.id, p_data.enumType.Name + "." + __enumValues[int.Parse(__value)]);
                    __getField += "\t\t\t\tcase EnvironmentType." + __environmentType + ": \n" + "\t\t\t\t\treturn " + __environmentType + "_" + p_data.id + ";\n\n";
                    break;
            }
        }

        __getField += "\t\t\t\tdefault: \n" + "\t\t\t\t\treturn " + GetDefaultValueForType(p_data.type).ToString().ToLower() + ";\n";

        __getField += "\t\t\t}\n\t\t}\n\t}\n";
   
        //case EnvironmentType:\n                    return SANDBOX_dataURIPath;\n\n                case EnvironmentType.PRODUCTION:\n                    return PRODUCTION_dataURIPath;\n\n                case EnvironmentType.TEST:\n                    return TEST_dataURIPath;\n\n                default:\n                    return null;\n            }\n        }\n    }");
        __code += __getField;


        __code += "}";

        System.IO.File.WriteAllText(__file, __code);
        Debug.Log("Created " + __file);
        AssetDatabase.Refresh();
    }

    private void SaveAllVariables()
    {
        foreach (string key in dictOfEnvVariablesByID.Keys)
        {
            EnvironmentVariableData __variable = dictOfEnvVariablesByID[key];

            if (__variable.wasModified)
            {
                __variable.wasModified = false;
                SaveEnvironmentVariable(__variable, true);
            }
        }
        
    }

    #endregion

    #region Utilities

    private void FindDefaultValueForType(VariableType p_type)
    {
        switch (p_type)
        {
            case VariableType.STRING:
                _defaultEmptyValue = DefaultStringValue;
                break;
            case VariableType.INT:
                _defaultEmptyValue = DefaultIntegerValue;
                break;
            case VariableType.FLOAT:
                _defaultEmptyValue = DefaultFloatValue;
                break;
            case VariableType.BOOL:
                _defaultEmptyValue = DefaultBooleanValue;
                break;
            case VariableType.ENUM:
                _defaultEmptyValue = DefaultEnumValue;
                break;
        }
    }

    private object GetDefaultValueForType(VariableType p_type)
    {
        switch (p_type)
        {
            case VariableType.STRING:
                return (object)DefaultStringValue;
            case VariableType.INT:
                return (object)DefaultIntegerValue;
            case VariableType.FLOAT:
                return (object)DefaultFloatValue;
            case VariableType.BOOL:
                return (object)DefaultBooleanValue;
            case VariableType.ENUM:
                return (object)DefaultEnumValue;

        }

        return null;
    }

    private VariableType GetVariableEnumType(Type p_typeOfValue)
    {
        switch (p_typeOfValue.ToString())
        {
            case "System.Int32":
                return VariableType.INT;
            case "System.Single":
                return VariableType.FLOAT;
            case "System.String":
                return VariableType.STRING;
            case "System.Boolean":
                return VariableType.BOOL;
            default:
                if (p_typeOfValue.IsEnum)
                {
                    return VariableType.ENUM;
                }
                else
                {
                    throw new ArgumentException("Case not implemented for: " + p_typeOfValue);
                }
        }   
    }

    private bool HasModifications()
    {
        return dictOfEnvVariablesByID.Any(f => f.Value.wasModified == true);
    }



    public static class NewColors
    {
        private static Color __outColor = new Color();

        public static Color LightSeaGreen
        {

            get
            {
                ColorUtility.TryParseHtmlString("#20B2AA", out __outColor);
                return __outColor;
            }
        }

        public static Color Teal
        {
            get
            {
                ColorUtility.TryParseHtmlString("#008080", out __outColor);
                return __outColor;
            }
        }

        public static Color Orange
        {
            get
            {
                ColorUtility.TryParseHtmlString("#FFA500", out __outColor);
                return __outColor;
            }
        }

        public static Color ForestGreen
        {
            get
            {
                ColorUtility.TryParseHtmlString("#228B22", out __outColor);
                return __outColor;
            }
        }

        public static Color SeaGreen
        {
            get
            {
                ColorUtility.TryParseHtmlString("#2E8B57", out __outColor);
                return __outColor;
            }
        }

        public static Color DarkGreen
        {
            get
            {
                ColorUtility.TryParseHtmlString("#006400", out __outColor);
                return __outColor;
            }
        }

        public static Color PaleGreen
        {
            get
            {
                ColorUtility.TryParseHtmlString("#98FB98", out __outColor);
                return __outColor;
            }
        }

        public static Color SpringGreen
        {
            get
            {
                ColorUtility.TryParseHtmlString("#00FF7F", out __outColor);
                return __outColor;
            }
        }

        public static Color LightSteelBlue
        {
            get
            {
                ColorUtility.TryParseHtmlString("#B0C4DE", out __outColor);
                return __outColor;
            }
        }

        public static Color PowderBlue
        {
            get
            {
                ColorUtility.TryParseHtmlString("#B0E0E6", out __outColor);
                return __outColor;
            }
        }

        public static Color FireBrick
        {
            get
            {
                ColorUtility.TryParseHtmlString("#B22222", out __outColor);
                return __outColor;
            }
        }
    }

    #endregion
}
