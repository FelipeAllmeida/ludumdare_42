using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Internal;
using UnityEngine.AI;

namespace Main
{
    public class Player : MonoBehaviour
    {
        private CommandController _commandController;
        private InputManager _inputManager;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Unit _unit;

        [SerializeField] private float _cameraSpeed = 0.05f;

	    // Use this for initialization
	    void Start ()
        {
            InitializeCommandController();
            InitializeInputManager();
        }

        // Update is called once per frame
        void Update ()
        {
            _commandController.UpdateCommands();
            _inputManager?.CheckInput();

            Vector3 __cameraMoveOffset = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                __cameraMoveOffset.z += _cameraSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                __cameraMoveOffset.z -= _cameraSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                __cameraMoveOffset.x -= _cameraSpeed * Time.deltaTime;
            }
            else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                __cameraMoveOffset.x += _cameraSpeed * Time.deltaTime;
            }

            _mainCamera.transform.position += __cameraMoveOffset;
        }

        #region Internal
        private void InitializeCommandController()
        {
            _commandController = new CommandController();
        }

        private void InitializeInputManager()
        {
            _inputManager = new InputManager();

            ListenInputManagerEvents(_inputManager);
        }

        private void ListenInputManagerEvents(InputManager p_inputManager)
        {
            p_inputManager.onMouseLeftClick += InputManager_OnMouseLeftClick;
            p_inputManager.onMouseRightClick += InputManager_OnMouseRightClick;
        }

        private void InputManager_OnMouseLeftClick(InputInfo p_inputInfo)
        {

        }

        private void InputManager_OnMouseRightClick(InputInfo p_inputInfo)
        {
            _commandController.StopCurrentCommand();
            _commandController.MoveTo(_unit, p_inputInfo.worldClickPoint);
        }
        #endregion
    }
}
