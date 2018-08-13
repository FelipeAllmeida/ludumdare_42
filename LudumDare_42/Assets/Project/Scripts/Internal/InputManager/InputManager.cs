using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internal
{

    public enum GestureType
    {
        DRAG,
        PINCH
    }

    public enum GesturePhaseType
    {
        START,
        UPDATE,
        FINISH
    }

    public class InputInfo
    {
        public GesturePhaseType phase;
        public GameObject hit;
        public Vector3 worldClickPoint;
        public Vector3 screenClickPoint;
    }

    public class InputManager
    {
        public event Action<InputInfo> onMouseLeftClick;
        public event Action<InputInfo> onMouseRightClick;

        private GestureType _currenteMouseGesture;

        private Dictionary<int, InputInfo> _dictInputs = new Dictionary<int, InputInfo>();

        private bool _isInputsEnabled = true;

        private float _currentFloor = 0f;

        public void EnableInputs(bool p_value)
        {
            _isInputsEnabled = p_value;
            if (p_value == false)
            {
                _dictInputs.Clear();
            }
        }

        public void SetCurrentFloor(int p_floor)
        {
            _currentFloor = (int)p_floor;
        }

        public void CheckInput()
        {
            HandleMouseInputs();
        }

        public Vector3 GetMousePosition()
        {
            return Input.mousePosition;
        }

        private void HandleMouseInputs()
        {
            if (Input.GetMouseButtonDown(0) == true)
            {
                HandleMouseClick(0, GesturePhaseType.START);
            }
            else if (Input.GetMouseButton(0) == true)
            {
                HandleMouseClick(0, GesturePhaseType.UPDATE);
            }
            else if (Input.GetMouseButtonUp(0) == true)
            {
                HandleMouseClick(0, GesturePhaseType.FINISH);
            }

            if (Input.GetMouseButtonDown(1) == true)
            {
                HandleMouseClick(1, GesturePhaseType.START);
            }
            else if (Input.GetMouseButton(1) == true)
            {
                HandleMouseClick(1, GesturePhaseType.UPDATE);
            }
            else if (Input.GetMouseButtonUp(1) == true)
            {
                HandleMouseClick(1, GesturePhaseType.FINISH);
            }
        }

        private void HandleMouseClick(int p_inputID, GesturePhaseType p_phase)
        {
            if (p_phase == GesturePhaseType.FINISH)
            {
                if (_dictInputs.ContainsKey(p_inputID))
                {
                    bool _isAllowedLayer = false;
                    UpdateInputInfo(p_inputID, p_phase, out _isAllowedLayer);
                    if (p_inputID == 0)
                    {
                        onMouseLeftClick?.Invoke(_dictInputs[p_inputID]);
                    }
                    else if (p_inputID == 1)
                    {
                        onMouseRightClick?.Invoke(_dictInputs[p_inputID]);
                    }

                    _dictInputs.Remove(p_inputID);
                }
            }
            else if (p_phase == GesturePhaseType.UPDATE)
            {
                bool _isAllowedLayer = false;
                UpdateInputInfo(p_inputID, p_phase, out _isAllowedLayer);
                if (p_inputID == 0)
                {
                    onMouseLeftClick?.Invoke(_dictInputs[0]);
                }
                else if (p_inputID == 1)
                {
                    onMouseRightClick?.Invoke(_dictInputs[1]);
                }
            }
            else if (p_phase == GesturePhaseType.START)
            {
                if (_dictInputs.ContainsKey(p_inputID) == false)
                {
                    bool _isAllowedLayer;
                    InputInfo __inputInfo = CreateInputInfo(p_phase, out _isAllowedLayer);
                    _dictInputs.Add(p_inputID, __inputInfo);
                    if (_isAllowedLayer == false) return;
                    if (p_inputID == 0)
                    {
                        onMouseLeftClick?.Invoke(__inputInfo);
                    }
                    else if (p_inputID == 1)
                    {
                        onMouseRightClick?.Invoke(__inputInfo);
                    }
                }
            }
        }

        private InputInfo CreateInputInfo(GesturePhaseType p_phase, out bool p_isInputAllowed)
        {
            p_isInputAllowed = false;

            InputInfo __inputInfo = new InputInfo();
            __inputInfo.phase = p_phase;

            RaycastHit __raycastHit = new RaycastHit();
            Ray __ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(__ray, out __raycastHit, 100f, ~(1 << LayerMask.NameToLayer("RangeCollider"))))
            {
                Debug.DrawRay(Camera.main.transform.position, __ray.direction * 50, Color.green, 1f);

                if (__raycastHit.collider.gameObject.layer == 0 || __raycastHit.collider.gameObject.layer == 9)
                {
                    p_isInputAllowed = true;
                }

                __inputInfo.hit = __raycastHit.collider.gameObject;
                __inputInfo.worldClickPoint = __raycastHit.point;
                __inputInfo.worldClickPoint.y = _currentFloor;
                __inputInfo.screenClickPoint = Input.mousePosition;
            }
            return __inputInfo;
        }

        private void UpdateInputInfo(int p_inputID, GesturePhaseType p_phase, out bool p_isInputAllowed)
        {
            p_isInputAllowed = false;
            if (_dictInputs.ContainsKey(p_inputID) == true)
            {
                _dictInputs[p_inputID].phase = p_phase;

                RaycastHit __raycastHit = new RaycastHit();
                Ray __ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(__ray, out __raycastHit, 100f, ~(1 << LayerMask.NameToLayer("RangeCollider"))))
                {
                    Debug.DrawLine(__ray.origin, __raycastHit.point);
                    if (__raycastHit.collider.gameObject.layer == 0 || __raycastHit.collider.gameObject.layer == 9)
                    {
                        p_isInputAllowed = true;
                    }

                    _dictInputs[p_inputID].hit = __raycastHit.collider.gameObject;
                    _dictInputs[p_inputID].worldClickPoint = __raycastHit.point;
                    _dictInputs[p_inputID].worldClickPoint.y = _currentFloor;
                    _dictInputs[p_inputID].screenClickPoint = Input.mousePosition;
                }
            }
        }
    }

}