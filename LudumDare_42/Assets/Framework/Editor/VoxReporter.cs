//Made by Felipe 'Tindão' Almeida
//Email: felipealmeida@voxstudio.com.br

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;


public class VoxReporter : EditorWindow
{
    private static VoxReporter _window;

    private static bool _loggedIn = false;
    private static string _user;

    private static string _currentScreenshotPath = string.Empty;

    private static int _bugID;
    private static int _screenshotCount = 1;
    private static int _initializationFocusCounter = 5;
    private static string _titleText;
    private static string _descriptionText;
    private static string _versionNumber = "Vox Reporter v_1.0 - Made by Felipe Almeida";

    private static List<Texture2D> _listScreenShots;

    private static Vector2 _scrollPos;

    private static bool _isInitialization = false;
    private static bool _createAnotherReport = false;
    private static bool _isSettings = false;
    private static bool _showScreenShots = false;

    [MenuItem("Vox/Reports/Reporter &r")]
    public static void ShowWindow()
    {
        _showScreenShots = false;
        LoadWindow();
    }

    [MenuItem("Vox/Reports/Screenshot Reporter &#r")]
    public static void ShowWindowWithScreenshot()
    {
        _showScreenShots = true;
        LoadWindow();
        VoxUtility.TakeScreenshot();
    }

    private static void LoadWindow()
    {
        _initializationFocusCounter = 5;

        _window = EditorWindow.GetWindow<VoxReporter>(false, "Reporter");
        //_window.titleContent.image = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Framework/Resources/VoxEditorData/VoxLogo32.png", typeof(Texture));
        float __screenWidth = 600f;
        float __screenHeight = (_showScreenShots == true) ? 440f : 280f;
        _window.position = new Rect((Screen.currentResolution.width * .5f) - __screenWidth/2f, (Screen.currentResolution.height * .5f) - __screenHeight/2f, __screenWidth, __screenHeight);
        _window.Focus();
        ResetWindow();
    }

    public void OnEnable()
    {
        ResetWindow();
    }

    public void Update()
    {
        if (_currentScreenshotPath != string.Empty && File.Exists(_currentScreenshotPath))
        {
            string __path = _currentScreenshotPath;
            _currentScreenshotPath = string.Empty;
            AddScreenshotToList(LoadScreenshot(__path));
        }
    }

    #region OnGUI
    private void OnGUI()
    {
        VerifyData();

        if (_loggedIn)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space(10f);
                if (_isSettings == true)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("User", EditorStyles.boldLabel, GUILayout.Width(55f));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Return")))
                        {
                            _isSettings = false;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        EditorGUILayout.SelectableLabel(_user);
                        if (GUILayout.Button(new GUIContent("Change User"), GUILayout.Width(100f)))
                        {
                            EditorPrefs.DeleteKey("VoxReporter");
                            _user = string.Empty;
                            _isSettings = false;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Bug Title", EditorStyles.boldLabel, GUILayout.Width(145f));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Settings")))
                        {
                            _isSettings = true;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        GUI.SetNextControlName("titleText");
                        _titleText = GUILayout.TextArea(_titleText);
                        if (_isInitialization == true)
                        {
                            GUI.FocusControl("titleText");

                            if (_initializationFocusCounter == 0)
                                _isInitialization = false;
                            else
                                _initializationFocusCounter--;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(10f);
                    
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Description", EditorStyles.boldLabel);
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        _descriptionText = GUILayout.TextArea(_descriptionText, GUILayout.Height(75f));
                    }
                    EditorGUILayout.EndHorizontal();

                    if (_showScreenShots == true)
                    {
                        GUILayout.Space(10f);

                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("Screenshot", EditorStyles.boldLabel);
                            GUILayout.FlexibleSpace();
                        }
                        EditorGUILayout.EndHorizontal();

                        if (_listScreenShots != null && _listScreenShots.Count > 0)
                        {
                            StartWindowScroller();
                            {
                                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                                {
                                    for (int i = 0; i < _listScreenShots.Count; i++)
                                    {
                                        GUILayout.Label(new GUIContent(_listScreenShots[i]), GUILayout.Width((float)_listScreenShots[i].width), GUILayout.Height((float)_listScreenShots[i].height));
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EndWindowScroller();
                        }
                        else
                        {
                            GUILayout.Label("Loading screenshot...");
                            GUILayout.Space(100f);
                        }
                    }

                    GUILayout.Space(10f);

                    _createAnotherReport = GUILayout.Toggle(_createAnotherReport, new GUIContent("Create Another"));

                    GUI.color = Color.green;
                    if (GUILayout.Button(new GUIContent("Submit (Enter)", "Save the report on the reports folder."), GUILayout.Height(50f)))
                    {
                        Submit();
                    }
                    GUI.color = Color.white;

                }
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    _user = EditorGUILayout.TextField(_user);
                }
                EditorGUILayout.EndHorizontal();

                GUI.color = Color.green;
                if (GUILayout.Button(new GUIContent("Save User")))
                {
                    Debug.Log("Save User: " + _user);
                    EditorPrefs.SetString("VoxReporter", _user);
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndVertical();
        }

        DrawVoxDebuggerInfo();
    }

    private static void DrawVoxDebuggerInfo()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(_versionNumber, EditorStyles.miniLabel);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void VerifyData()
    {
        if (_bugID == 0)
        {
            _bugID = UnityEngine.Random.Range(1, 99999);
        }
       
        string __tempUser = EditorPrefs.GetString("VoxReporter");
        bool __loggedInPreviousState = _loggedIn;
        _loggedIn = (__tempUser != string.Empty);
        if (_loggedIn == true && __loggedInPreviousState != _loggedIn)
        {
            _user = __tempUser;
        }
    }

    private void UpdateScreenshotCount()
    {
        if (_screenshotCount != _listScreenShots.Count)
        {
            List<Texture2D> __listTemp = new List<Texture2D>(_listScreenShots);

            _listScreenShots.Clear();

            for (int i = 0; i < _screenshotCount; i ++)
            {
                if (i <= __listTemp.Count - 1)
                {
                    _listScreenShots.Add(__listTemp[i]);
                }
                else
                {
                    _listScreenShots.Add(null);
                }
            }
        }
    }

    private void AddScreenshotToList(Texture2D __texture2D)
    {
        int __index = _listScreenShots.FindIndex(x => x == null);

        if (__index > -1)
        {
            _listScreenShots[__index] = __texture2D;
        }
        else
        {
            _screenshotCount++;
            _listScreenShots.Add(__texture2D);
        }
    }

    private static void ResetWindow()
    {
        if (_window == null)
        {
            _window = EditorWindow.GetWindow<VoxReporter>(false, "Reporter");
        }

        VoxUtility.onTakeScreenshot -= HandleOnTakeScreenshot;
        VoxUtility.onTakeScreenshot += HandleOnTakeScreenshot;

        _isInitialization = true;

        _screenshotCount = 1;

        _bugID = UnityEngine.Random.Range(1, 99999);
        _titleText = string.Empty;
        _descriptionText = string.Empty;
        _listScreenShots = new List<Texture2D>();
    }

    private void Submit()
    {
        if (_titleText == string.Empty)
        {
            if (EditorUtility.DisplayDialog("ERROR", "Report must have a title", "Understood"))
            {
                return;
            }
        }

        Report __report = new Report();
        __report.id = _bugID.ToString();
        __report.title = _titleText;
        __report.description = _descriptionText;
        __report.user = _user;

        if (_listScreenShots.Count == 1 && _listScreenShots[0] == null)
        {
            __report.listScreenShots = null;
        }
        else
        {
            __report.listScreenShots = new List<string>(_listScreenShots.Count);
            for (int i = 0; i < _listScreenShots.Count; i ++)
            {
                string __fileName = _listScreenShots[i].name;
                if (__fileName.Contains(".png") == false)
                {
                    __fileName += ".png";
                }
                __report.listScreenShots.Add(__fileName);
            }
        }

        Action __callbackWriteData = () =>
        {
            StreamWriter __fieldDataWriter = File.CreateText(Application.dataPath + "/VoxReports/" + __report.id + ".json");
            __fieldDataWriter.WriteLine(JsonUtility.ToJson(__report, true));
            __fieldDataWriter.Close();
            ResetWindow();
            if (_showScreenShots)
            {
                VoxUtility.TakeScreenshot();
            }

            if (_createAnotherReport == false)
            {
                AssetDatabase.Refresh();
                _window.Close();
            }
            
        };

        if (Directory.Exists(Application.dataPath + "/VoxReports"))
        {
            __callbackWriteData();
        }
        else
        {
            if (Directory.Exists(Application.dataPath + "/VoxReports") == false)
                Directory.CreateDirectory(Application.dataPath + "/VoxReports");

            __callbackWriteData();
        }
    }

    private static void HandleOnTakeScreenshot(string p_fileName)
    {
        _currentScreenshotPath = Application.dataPath + "/Framework/Screenshot/" + p_fileName;
        AssetDatabase.Refresh(ImportAssetOptions.Default);
        _window.Repaint();
    }

    private Texture2D LoadScreenshot(string p_path)
    {
        byte[] __bytes = File.ReadAllBytes(p_path);

        string[] __arrayPath = p_path.Split('/');

        Texture2D __texture = new Texture2D(100, 100);
        __texture.name = __arrayPath[__arrayPath.Length - 1];
        __texture.LoadImage(__bytes);

        float __aspectRatioXtoY = (float)__texture.width / (float)__texture.height;
        float __newWidth, __newHeight;
        if (__aspectRatioXtoY > 1f)
        {
            __newWidth = 100f;
            __newHeight = 100f / __aspectRatioXtoY;
        }
        else
        {
            __newHeight = 100f;
            __newWidth = 100f * __aspectRatioXtoY;
        }

        __texture.Resize((int)__newWidth, (int)__newHeight);
        return __texture;
    }

    private static void StartWindowScroller()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
    }

    private static void EndWindowScroller()
    {
        EditorGUILayout.EndScrollView();
    }
    #endregion
}
