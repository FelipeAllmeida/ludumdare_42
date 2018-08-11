using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VoxUtility
{
    #region Events
    public static Action<string> onTakeScreenshot;
    #endregion

    [MenuItem("Vox/Utility/InputSimulator/Activate")]
    public static void ActivateInputSimulator()
    {
        string __path = Application.dataPath + "/Framework/Resources/VoxUtility/InputSimulation.json";

        System.IO.StreamWriter __fieldDataWriter = System.IO.File.CreateText(__path);
        InputSimulation __a = new InputSimulation();
        __a.enabled = true;
        __fieldDataWriter.WriteLine(JsonUtility.ToJson(__a, true));
        __fieldDataWriter.Close();
        AssetDatabase.Refresh();
    }

    [MenuItem("Vox/Utility/InputSimulator/Deactivate")]
    public static void DeactivateInputSimulator()
    {
        string __path = Application.dataPath + "/Framework/Resources/VoxUtility/InputSimulation.json";

        System.IO.StreamWriter __fieldDataWriter = System.IO.File.CreateText(__path);
        InputSimulation __a = new InputSimulation();
        __a.enabled = false;
        __fieldDataWriter.WriteLine(JsonUtility.ToJson(__a, true));
        __fieldDataWriter.Close();
        AssetDatabase.Refresh();
    }

    #region Screenshot
    [MenuItem("Vox/Utility/Take Screenshot &s")]
    public static void TakeScreenshot()
    {
        EditorApplication.ExecuteMenuItem("Window/Game");

        int __id = -1;

        if (System.IO.Directory.Exists(Application.dataPath + "/Framework/Screenshot") == false)
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Framework/Screenshot");
        }

        bool __isUnique = false;
        do
        {
            __id = UnityEngine.Random.Range(0, 999999);

            if (System.IO.File.Exists(Application.dataPath + "/Framework/Screenshot/" + __id.ToString("000000") + ".png") == false)
            {
                __isUnique = true;
            }
        }
        while (__isUnique == false);

        string __path = Application.dataPath + "/Framework/Screenshot/" + __id.ToString("000000") + ".png";
        Debug.Log(__path);
        ScreenCapture.CaptureScreenshot(__path);
        string __fileName = __id.ToString("000000") + ".png";

        if (onTakeScreenshot != null) onTakeScreenshot(__fileName);
    }

    public static string[] GetAllScreenshotPaths()
    {
        return System.IO.Directory.GetFiles(Application.dataPath + "/Framework/Screenshot");
    }

    public static Texture2D GetScreenshot(string p_fileName)
    {
        string __path = "Assets/Framework/Screenshot/" + p_fileName;
        return (Texture2D)AssetDatabase.LoadAssetAtPath(__path, typeof(Texture2D));
    }
    #endregion
}
