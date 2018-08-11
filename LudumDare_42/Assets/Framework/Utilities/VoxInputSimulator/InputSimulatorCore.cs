using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SimulatedFingerStateType
{
    NONE,
    BEGIN,
    UPDATE,
    FINISH
}

public class InputSimulatorCore : MonoBehaviour
{
    private static InputSimulatorCore _instance;
    /// <summary>
    /// Return the dictionary with all the supported number of fingers, understood that this dictionary always contains 5 fingers,
    /// to get the amount of active fingers use fingersCount.
    /// </summary>
    public static Dictionary<int, SimulatedFinger> fingers
    {
        get
        {
            return _uiInputSimulator.fingers;
        }
    }

    public static List<int> activeFingersID
    {
        get
        {
            List<int> __activeFingers = null;
            foreach (var k in fingers)
            {
                if (k.Value.phase != SimulatedFingerStateType.NONE)
                {
                    if (__activeFingers == null) __activeFingers = new List<int>();
                    __activeFingers.Add(k.Key);
                }
            }
            return __activeFingers;
        }
    }
    /// <summary>
    /// Return the number of active fingers.
    /// </summary>
    public static int fingersCount
    {
        get
        {
            int __count = 0;
            foreach(SimulatedFinger __simulatedFinger in fingers.Values)
            {
                if (__simulatedFinger.phase != SimulatedFingerStateType.NONE)
                {
                    __count++;
                }
            }
            return __count;
        }
    }
    /// <summary>
    /// Get the current screen position if you only have one finger, or the average if you have two or more fingers.
    /// </summary>
    public static Vector3 touchScreenPosition
    {
        get
        {
            if (fingers == null)
                return Vector3.zero;
            else
            {
                Vector3 __touchScreenPosition = new Vector3(Screen.width/2f, Screen.height/2f, 0f);
                foreach(SimulatedFinger __simulatedFinger in fingers.Values)
                {
                    if (__simulatedFinger.phase != SimulatedFingerStateType.NONE)
                        __touchScreenPosition += __simulatedFinger.position;
                }
                return __touchScreenPosition / fingers.Count;
            }
        }
    }

    public static Vector3 touchPosition
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(touchScreenPosition);
        }
    }

    private static UIInputSimulatorProvider _uiInputSimulator;
    private static WorldInputSimulatorProvider _worldInputSimulator;

    private static bool _isSimulatingLoaded = false;

    private static string _dataPath = "VoxUtility/InputSimulation";

    private static bool _isSimulating;
    public static bool isSimulating
    {
        get
        {
            if (_isSimulatingLoaded == false)
            {
                _isSimulatingLoaded = true;

                TextAsset __fieldDataJson = Resources.Load<TextAsset>(_dataPath) as TextAsset;

                if (__fieldDataJson == null)
                    throw new System.ArgumentException("InputSimulation.json does not exist");

                _isSimulating = JsonUtility.FromJson<InputSimulation>(__fieldDataJson.text).enabled;
            }
            return _isSimulating;
        }
    }

#region Engine Callbacks	
    void Update()
    {
        if (_isSimulating == false) return;

        _uiInputSimulator.AUpdate();

        if (Input.GetKey(KeyCode.T))
        {
            SimulateMouseTap(0, Input.mousePosition);//new Vector2(300f, 200f));//new Vector3(372f, 18f, 0f));
        }

        if (Input.GetKey(KeyCode.P))
        {
            SimulateMousePress(1, Input.mousePosition);//new Vector2(300f, 270f));
        }

        if (Input.GetKey(KeyCode.R))
        {
            SimulateMouseRelease(1, Input.mousePosition);//new Vector2(300f, 270f));
        }
    }

    void OnGUI()
    {
        if (_isSimulating == false) return;
        GUILayout.Space(350f);
        GUILayout.TextArea("Current Mouse Position: " +Input.mousePosition);
        if (fingers != null)
        {
            GUILayout.TextArea("Finger Count: " + fingersCount);
            GUILayout.Space(15f);
            foreach (var k in fingers)
            {
                if (activeFingersID != null && activeFingersID.Contains(k.Key))
                    GUILayout.TextArea(k.Key.ToString() + " | phase: " +k.Value.phase.ToString() + " | isDown: " +((k.Value.phase != SimulatedFingerStateType.NONE) ? "true" : "false") + " | position " +k.Value.position + " | delta " +k.Value.deltaPosition);
            }
        }
    }
#endregion

    public static void Initialize()
    {
        if (isSimulating == false)
        {
            Debug.Log("Input Simulation is disabled. To use just activate it on VoxUtility.");
            return;
        }

        if (_instance == null)
        {
            _instance = new GameObject("InputSimulatorCore", typeof(InputSimulatorCore)).GetComponent<InputSimulatorCore>();

            DontDestroyOnLoad(_instance.gameObject);

            _uiInputSimulator = _instance.gameObject.AddComponent<UIInputSimulatorProvider>();
            _uiInputSimulator.Initialize();
        }
    }

    public static void SimulateMouseTap(int p_id, Vector3 p_position)
    {
        CheckAndHandleIfInputSimulatorIsEnabled();
        _uiInputSimulator.SimulateMouseTap(p_id, p_position);
        if (_worldInputSimulator != null) _worldInputSimulator.SimulateMouseTap(p_id, p_position);
    }

    public static void SimulateMousePress(int p_id, Vector3 p_position)
    {
        CheckAndHandleIfInputSimulatorIsEnabled();
        _uiInputSimulator.SimulateMousePress(p_id, p_position);
        if (_worldInputSimulator != null) _worldInputSimulator.SimulateMousePress(p_id, p_position);
    }

    public static void SimulateMouseRelease(int p_id, Vector3 p_position)
    {
        CheckAndHandleIfInputSimulatorIsEnabled();
        _uiInputSimulator.SimulateMouseRelease(p_id, p_position);
        if (_worldInputSimulator != null) _worldInputSimulator.SimulateMouseRelease(p_id, p_position);
    }

    public static void AddUICanvas(GameObject p_canvas)
    {
        if (isSimulating == false) return;
        _uiInputSimulator.AddCanvas(p_canvas);
    }

    public static void SetWorldInputSimulatorProvider(WorldInputSimulatorProvider p_worldInputSimulator)
    {
        if (isSimulating == false) return;
        _worldInputSimulator = p_worldInputSimulator;
    }

    private static void CheckAndHandleIfInputSimulatorIsEnabled()
    {
        if (isSimulating == false)
        {
            throw new System.ArgumentException("Input Simulator has not been initialized.");
        }
    }
}

public class SimulatedFinger
{
    public SimulatedFinger()
    {
        phase = SimulatedFingerStateType.NONE;
    }

    public SimulatedFingerStateType phase;
    public Vector3 position;
    public Vector3 deltaPosition;
}


public struct InputSimulation
{
    public bool enabled;
}