using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vox;

public class WorldInputSimulatorProvider : FGInputProvider
{
    protected Dictionary<int, SimulatedFinger> _dictFingers;

    #region Input Simulator Data
    protected int _maxTouches = 5;
    #endregion

    #region Private Data
    protected TimerNode _tapTimerNodule;
    #endregion

    #region Engine Callbacks
    void Start()
    {
        InitializeSimulatedFingers();
    }

    void Update()
    {
        UpdateSimulatedInputs();
    }

    /*void OnGUI()
	{
		GUILayout.TextArea("Current Mouse Position: " + Input.mousePosition);
		if (_dictFingers != null)
		{
			GUILayout.TextArea("Finger Count: " + _dictFingers.Count);
			GUILayout.Space(15f);
			foreach (var k in _dictFingers)
			{
				GUILayout.TextArea(k.Key.ToString() + " | phase: " + k.Value.phase.ToString() + " | isDown: " + ((k.Value.phase != SimulatedFingerStateType.NONE) ? "true" : "false"));
			}
		}
	}*/
    #endregion

    #region Handle Inputs
    protected virtual void UpdateSimulatedInputs()
    {
        foreach (var k in _dictFingers)
        {
            switch (_dictFingers[k.Key].phase)
            {
                case SimulatedFingerStateType.BEGIN:
                    k.Value.phase = SimulatedFingerStateType.UPDATE;
                    break;
                case SimulatedFingerStateType.UPDATE:
                    break;
                case SimulatedFingerStateType.FINISH:
                    k.Value.phase = SimulatedFingerStateType.NONE;
                    break;
            }
        }
    }
    #endregion

    #region Simulation Request Methods
    public void SimulateMouseTap(int p_id, Vector3 p_position)
    {
        PressFinger(p_id, p_position);
        if (_tapTimerNodule != null) _tapTimerNodule.Cancel();
        _tapTimerNodule = Timer.WaitSeconds(0.1f, delegate
        {
            ReleaseFinger(p_id, p_position);
        });
    }

    public void SimulateMousePress(int p_id, Vector3 p_position)
    {
        PressFinger(p_id, p_position);
    }

    public void SimulateMouseRelease(int p_id, Vector3 p_position)
    {
        ReleaseFinger(p_id, p_position);
    }
    #endregion

    #region Utility
    protected void InitializeSimulatedFingers()
    {
        _dictFingers = new Dictionary<int, SimulatedFinger>();
        for (int i = 0; i < _maxTouches; i++)
        {
            _dictFingers.Add(i, new SimulatedFinger());
        }
    }

    protected void PressFinger(int p_id, Vector3 p_position)
    {
        if (_dictFingers.ContainsKey(p_id) == false) return;

        if (_dictFingers[p_id].phase == SimulatedFingerStateType.NONE)
        {
            _dictFingers[p_id].phase = SimulatedFingerStateType.BEGIN;
            _dictFingers[p_id].position = p_position;
        }
        else
        {
            _dictFingers[p_id].position = p_position;
        }
    }

    protected void ReleaseFinger(int p_id, Vector3 p_position)
    {
        if (_dictFingers.ContainsKey(p_id) && _dictFingers[p_id].phase != SimulatedFingerStateType.FINISH && _dictFingers[p_id].phase != SimulatedFingerStateType.NONE)
        {
            _dictFingers[p_id].phase = SimulatedFingerStateType.FINISH;
            _dictFingers[p_id].position = p_position;
        }
    }
    #endregion

    #region Overriden Methods
    public override void GetInputState(int p_fingerID, out bool o_down, out Vector2 o_position)
    {
        o_down = false;
        o_position = Vector2.zero;

        if (_dictFingers[p_fingerID].phase != SimulatedFingerStateType.NONE)
        {
            o_down = true;
            o_position = _dictFingers[p_fingerID].position;
        }
    }

    public override int MaxSimultaneousFingers
    {
        get { return _maxTouches; }
    }
    #endregion
}
