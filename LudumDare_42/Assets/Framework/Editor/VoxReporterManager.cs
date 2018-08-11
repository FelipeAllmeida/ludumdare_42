using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;



public class VoxReporterManager : EditorWindow
{
    #region Class
    public class UserReportData
    {
        public UserReportData() { listReportData = new List<ReportData>(); }
        public List<ReportData> listReportData;
        public bool isUnfolded = false;
    }
    [Serializable]
    public class ReportData
    {
        private ReportData() { }
        public ReportData(string p_id, string p_user, string p_title, string p_description, string p_path, List<Texture2D> p_listScreenshots)
        {
            _id = p_id;
            user = p_user;
            title = p_title;
            description = p_description;
            path = p_path;
            listScreenshots = p_listScreenshots;
        }
        private string _id;
        public string id { get { return _id; }}
        public string user;
        public string title;
        public string description;
        public List<Texture2D> listScreenshots;
        public string path;
        public bool isUnfolded = false;
        public Vector2 scrollScreenshots;

    }

    #endregion
    private static VoxReporterManager _window;

    private static Dictionary<string, UserReportData> _dictUserReports;

    private static Vector2 _scrollVertical;

    private GUIStyle _styleHtml = new GUIStyle();

    private static string _versionNumber = "Vox Report Manager v_1.0 - Made by Felipe Almeida";


    [MenuItem("Vox/Reports/Manager")]
    public static void ShowWindow()
    {
        _window = EditorWindow.GetWindow<VoxReporterManager>(false, "R. Manager");
        //_window.titleContent.image = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Framework/Resources/VoxEditorData/VoxLogo32.png", typeof(Texture));
        _window.position = new Rect(150f, 150f, 600f, 500f);

        LoadReportData();
    }

    private void OnEnable()
    {
        _styleHtml.richText = true;
        LoadReportData(true);
    }

    public void OnGUI()
    {
        DrawUserReports();
    }

    private void DrawUserReports()
    {
        GUILayout.Space(10f);

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(10f);
                GUILayout.Label("<b><size=12><color=#000000>Report Manager</color></size></b>", _styleHtml);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Force Reload"), GUILayout.Width(90f)))
                {
                    LoadReportData(true);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4f);
        }
        EditorGUILayout.EndVertical();

        _scrollVertical = EditorGUILayout.BeginScrollView(_scrollVertical, GUIStyle.none, GUI.skin.verticalScrollbar);

        if (_dictUserReports.Keys.Count == 0)
        {
            GUILayout.Label("No Reports To Display");
        }
        else
        {
            foreach (string __user in _dictUserReports.Keys)
            {
                UserReportData __userReportData = _dictUserReports[__user];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        __userReportData.isUnfolded = EditorGUILayout.Foldout(__userReportData.isUnfolded, string.Empty);
                        GUILayout.Space(-40);
                        GUILayout.Label("<b><size=12><color=#000000> " + __user + " </color></size></b>", _styleHtml);
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(4f);

                    if (__userReportData.isUnfolded)
                    {
                        for (int i = 0; i < __userReportData.listReportData.Count; i++)
                        {
                            GUI.color = (i % 2 == 0) ? new Color(216f / 255f, 216f / 255f, 216f / 255f) : Color.white;

                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            {
                                GUI.color = Color.white;
                                ReportData __reportData = __userReportData.listReportData[i];

                                EditorGUILayout.BeginHorizontal();
                                {
                                    __reportData.isUnfolded = EditorGUILayout.Foldout(__reportData.isUnfolded, string.Empty);
                                    GUILayout.Space(-30f);
                                    GUILayout.Label("<size=12>" + __reportData.title + "</size>", _styleHtml);

                                    GUILayout.FlexibleSpace();

                                    if (GUILayout.Button("X", GUILayout.Width(20f)))
                                    {
                                        if (EditorUtility.DisplayDialog("Delete " + __reportData.title, "You're sure you wish to delete this report?", "Yes", "Cancel"))
                                        {
                                            RemoveReportData(__reportData);
                                            return;
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                GUILayout.Space(4f);

                                if (__reportData.isUnfolded == true)
                                {
                                    GUILayout.Space(10f);
                                    if (__reportData.description != string.Empty)
                                    {
                                        EditorGUILayout.SelectableLabel(__reportData.description);
                                    }

                                    if (__reportData.listScreenshots != null && __reportData.listScreenshots.Count > 0)
                                    {
                                        __reportData.scrollScreenshots = EditorGUILayout.BeginScrollView(__reportData.scrollScreenshots, GUI.skin.horizontalScrollbar, GUIStyle.none);
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            for (int j = 0; j < __reportData.listScreenshots.Count; j++)
                                            {
                                                GUILayout.Label(new GUIContent(__reportData.listScreenshots[j]), GUILayout.Width((float)__reportData.listScreenshots[j].width), GUILayout.Height((float)__reportData.listScreenshots[j].height));
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.EndScrollView();
                                    }
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndScrollView();

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

    #region Save/Load/Delete
    private static void LoadReportData(bool p_clearCurrent = false)
    {
        if (_dictUserReports == null || p_clearCurrent == true)
        {
            _dictUserReports = new Dictionary<string, UserReportData>();
        }

        string[] __arrayFiles = Directory.GetFiles(Application.dataPath + "/VoxReports");

        string __jsonContent;

        for (int i = 0; i < __arrayFiles.Length; i ++)
        {
            if (__arrayFiles[i].Contains(".meta")) continue;

            __jsonContent = string.Empty;

            string __fileName = __arrayFiles[i].Replace(Application.dataPath + "/VoxReports", "").Replace(".json", "");

            using (StreamReader reader = new StreamReader(__arrayFiles[i]))
            {
                __jsonContent = reader.ReadToEnd();
            }

            Report __report = null;

            if (__jsonContent != string.Empty)
            {
                __report = (Report)JsonUtility.FromJson(__jsonContent, typeof(Report));
            }

            if (__report != null)
            {
                Action __addReport = () => 
                {
                    if (_dictUserReports[__report.user].listReportData.Exists(x => x.id == __report.id) == false)
                    {
                        List<Texture2D> __listScreenshoots = LoadScreenshots(__report.listScreenShots);
                        ReportData __reportData = new ReportData(__report.id, __report.user, __report.title, __report.description, __arrayFiles[i], __listScreenshoots);
                        _dictUserReports[__report.user].listReportData.Add(__reportData);
                    }
                };

                if (_dictUserReports.ContainsKey(__report.user) == true)
                {
                    __addReport();
                }
                else
                {
                    UserReportData __reportData = new UserReportData();
                    _dictUserReports.Add(__report.user, __reportData);
                    __addReport();
                }
            }
        }
    }

    private static List<Texture2D> LoadScreenshots(List<string> p_listScreenshootFileNames)
    {
        List<Texture2D> __listScreenshoot = new List<Texture2D>();

        for (int i = 0; i < p_listScreenshootFileNames.Count; i ++)
        {
            Texture2D __texture = LoadScreenshot(Application.dataPath + "/Framework/Screenshot/" + p_listScreenshootFileNames[i]);
            if (__texture != null)
            {
                __listScreenshoot.Add(__texture);
            }
        }

        return __listScreenshoot;
    }

    private static Texture2D LoadScreenshot(string p_path)
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

    private void RemoveReportData(ReportData p_reportData)
    {
        if (_dictUserReports != null)
        {
            if (_dictUserReports.ContainsKey(p_reportData.user))
            {
                if(_dictUserReports[p_reportData.user].listReportData.Remove(p_reportData))
                {
                    File.Delete(p_reportData.path);
                    File.Delete(p_reportData.path + ".meta");
                }
            }
        }
    }
    #endregion
}
