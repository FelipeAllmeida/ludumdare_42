using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Vox;

public class UIInputSimulatorProvider : MonoBehaviour
{
    #region Public Data
    public Dictionary<int, SimulatedFinger> fingers
    {
        get
        {
            return _dictFingers;
        }
    }
    #endregion
    
    #region Private Data
    private List<GameObject> _listCanvas = new List<GameObject>();
    private Dictionary<int, SimulatedFinger> _dictFingers;
    protected TimerNode _tapTimerNodule;
    #endregion

    #region Input Simulator Data
    protected int _maxTouches = 5;
    #endregion

    #region Engine Callbacks
    public void Initialize()
    {
        InitializeSimulatedFingers();
    }

    public void AUpdate()
    {
        UpdateSimulatedInputs();
    }
    #endregion

    #region Handle Inputs
    protected void UpdateSimulatedInputs()
    {
        foreach (var k in _dictFingers)
        {
            switch (_dictFingers[k.Key].phase)
            {
                case SimulatedFingerStateType.BEGIN:
                    SimulateMouseEvents(k.Value.phase, k.Value.position, RaycastAllCanvasAtScreenPosition(k.Value.position));
                    k.Value.phase = SimulatedFingerStateType.UPDATE;
                    break;
                case SimulatedFingerStateType.UPDATE:
                    SimulateMouseEvents(k.Value.phase, k.Value.position, RaycastAllCanvasAtScreenPosition(k.Value.position));

                    break;
                case SimulatedFingerStateType.FINISH:
                    SimulateMouseEvents(k.Value.phase, k.Value.position, RaycastAllCanvasAtScreenPosition(k.Value.position));

                    k.Value.position = Vector3.zero;
                    k.Value.deltaPosition = Vector3.zero;
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
    public void AddCanvas(GameObject p_canvas)
    {
        if (p_canvas == null)
        {
            throw new ArgumentNullException("p_canvas cannot be null");
        }

        if (_listCanvas.Contains(p_canvas) == false)
        {
            _listCanvas.Add(p_canvas);
        }
    }

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
            _dictFingers[p_id].deltaPosition = Vector3.zero;
        }
        else
        {
            _dictFingers[p_id].deltaPosition = p_position - _dictFingers[p_id].position;
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

    private List<RaycastResult> RaycastAllCanvasAtScreenPosition(Vector3 p_screenPosition)
    {
        List<GameObject> __listNullCanvas = new List<GameObject>();
        List<RaycastResult> ___listTotalRaycastResult = new List<RaycastResult>();
        for (int i = 0; i < _listCanvas.Count; i++)
        {
            if (_listCanvas[i] != null)
            {
                if (_listCanvas[i].activeSelf == true)
                {
                    GraphicRaycaster __graphicRaycaster = _listCanvas[i].GetComponent<GraphicRaycaster>();
                    if (__graphicRaycaster != null)
                    {
                        List<RaycastResult> __listResults = new List<RaycastResult>();

                        PointerEventData __pointerEventData = new PointerEventData(EventSystem.current);
                        __pointerEventData.position = p_screenPosition;
                        __graphicRaycaster.Raycast(__pointerEventData, __listResults);
                        ___listTotalRaycastResult.AddRange(__listResults);
                    }
                }
            }
            else
            {
                __listNullCanvas.Add(_listCanvas[i]);
            }
        }

        for (int i = 0; i < __listNullCanvas.Count; i++)
        {
            _listCanvas.Remove(__listNullCanvas[i]);
        }
        return ___listTotalRaycastResult;
    }

    private void SimulateMouseEvents(SimulatedFingerStateType p_gestureStateType, Vector3 p_screenPosition, List<RaycastResult> p_listRaycastResult)
    {
        PointerEventData __newPointerEventData = new PointerEventData(null);
        __newPointerEventData.position = p_screenPosition;
        __newPointerEventData.pressPosition = p_screenPosition;
        for (int i = 0; i < p_listRaycastResult.Count; i++)
        {
            GameObject __go = (ExecuteEvents.CanHandleEvent<ISelectHandler>(p_listRaycastResult[i].gameObject) == true) ? p_listRaycastResult[i].gameObject : p_listRaycastResult[i].gameObject.transform.parent.gameObject;
            __newPointerEventData.pointerPressRaycast = p_listRaycastResult[i];

            switch (p_gestureStateType)
            {
                case SimulatedFingerStateType.BEGIN:
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.selectHandler);
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.pointerEnterHandler);
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.pointerDownHandler);
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.initializePotentialDrag);
                    break;
                case SimulatedFingerStateType.UPDATE:
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.dragHandler);
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.scrollHandler);
                    break;
                case SimulatedFingerStateType.FINISH:
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.pointerClickHandler);

                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.pointerExitHandler);
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.endDragHandler);
                    ExecuteEvents.Execute(__go, __newPointerEventData, ExecuteEvents.deselectHandler);
                    break;
            }
        }
    }
    #endregion
}
