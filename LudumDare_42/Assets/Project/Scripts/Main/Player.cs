using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Internal;
using UnityEngine.AI;
using Vox;
using Internal.Commands;

namespace Main
{
    public class Player : MonoBehaviour
    {
        private CommandQuery _commandQuery;
        private InputManager _inputManager;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private InteractionMenu _interactionMenu;
        [SerializeField] private Unit _unit;

        [SerializeField] private float _cameraSpeed = 0.05f;

        public Camera Camera { get { return _mainCamera; } }

	    // Use this for initialization
	    void Start ()
        {
            InitializeCommandController();
            InitializeInputManager();
            InitializeInteractionMenu();
        }

        // Update is called once per frame
        void Update ()
        {
            _commandQuery.UpdateQuery();
            _inputManager?.CheckInput();

            CheckCameraInputs();
        }

        #region Internal
        private void InitializeInteractionMenu()
        {
            _interactionMenu.SetCamera(Camera);
            ListenInteractionMenuEvents(_interactionMenu);
        }

        private void InitializeCommandController()
        {
            _commandQuery = new CommandQuery();
        }

        private void InitializeInputManager()
        {
            _inputManager = new InputManager();

            ListenInputManagerEvents(_inputManager);
        }

        private void ListenInteractionMenuEvents(InteractionMenu p_interactionMenu)
        {
            p_interactionMenu.onClickAction += InteractionMenu_OnClickAction;
        }

        private void InteractionMenu_OnClickAction(object p_source, OnClickActionEventArgs p_args)
        {
            switch(p_args.ObjectAction)
            {
                case ObjectAction.Interact:
                    _commandQuery.AddCommand(new MoveCommand(_unit, p_args.MapObject.transform.position, (object p_commandSource, CommandCallbackEventArgs p_commandArgs) =>
                    {
                        p_args.MapObject.Interact();
                    }));
                    break;
                case ObjectAction.Cancel:
                    break;
            }
        }

        private void ListenInputManagerEvents(InputManager p_inputManager)
        {
            p_inputManager.onMouseLeftClick += InputManager_OnMouseLeftClick;
            p_inputManager.onMouseRightClick += InputManager_OnMouseRightClick;
        }

        private void InputManager_OnMouseLeftClick(InputInfo p_inputInfo)
        {
            if (_interactionMenu.IsOpen == false)
            {
                MapObject __mapObject = p_inputInfo.hit.GetComponent<MapObject>();
                if ((IActor)__mapObject != null)
                {
                    _interactionMenu.Open(__mapObject);
                }
            }
        }

        private void InputManager_OnMouseRightClick(InputInfo p_inputInfo)
        {
            _commandQuery.ClearQuery();
            _commandQuery.AddCommand(new MoveCommand(_unit, p_inputInfo.worldClickPoint));
        }

        private void CheckCameraInputs()
        {
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
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                __cameraMoveOffset.x += _cameraSpeed * Time.deltaTime;
            }

            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _mainCamera.transform.position + __cameraMoveOffset, _cameraSpeed);
        }
        #endregion
    }
}
