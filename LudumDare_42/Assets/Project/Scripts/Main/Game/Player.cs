using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Internal;
using UnityEngine.AI;
using Vox;
using Internal.Commands;
using Main.Game.UI;
using System;
using Main.Game.Itens;
using Internal.Audio;

namespace Main.Game
{
    [RequireComponent(typeof(InteractionMenu))]
    public class Player : MonoBehaviour
    {
        public event Action onPause;

        private CommandQuery _commandQuery;
        private InputManager _inputManager;

        [SerializeField] private Animator _animator;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private InteractionMenu _interactionMenu;
        [SerializeField] private Unit _unit;
        [SerializeField] private PlayerUI _playerUI;
        public PlayerUI UI { get { return _playerUI; } }

        [SerializeField] private float _cameraSpeed = 0.05f;

        private bool _isRightClickMoving = false;

        public bool IsImmerse { get; private set; }

	    // Use this for initialization
	    public void Initialize()
        {
            _playerUI.Initialize();

            InitializeCommandController();
            InitializeInputManager();
            InitializeInteractionMenu();

            ResetCameraPosition(true);

            ListenUnitEvents(_unit);
        }

        // Update is called once per frame
        public void UpdatePlayer(float p_oxygen, float p_energy, TimeSpan p_timeLeft)
        {
            _playerUI.OxygenBar.SetFillProgress(p_oxygen);
            _playerUI.EnergyBar.SetFillProgress(p_energy);
            _playerUI.Clock.SetClockTime(p_timeLeft);
            _commandQuery.UpdateQuery();
            _inputManager?.CheckInput();

            CheckCameraInputs();
            CheckInputs();
        }

        public void EnableInputs(bool p_value)
        {
            _inputManager.EnableInputs(p_value);
        }

        public void ResumeCurrentCommand()
        {
            _commandQuery.Resume();
        }

        public void PauseCurrentCommand()
        {
            _commandQuery.Pause();
        }

        #region Internal
        private void InitializeInteractionMenu()
        {
            _interactionMenu.SetCamera(_mainCamera);
            ListenInteractionMenuEvents(_interactionMenu);
        }

        private void InitializeCommandController()
        {
            _commandQuery = new CommandQuery();
        }

        private void InitializeInputManager()
        {
            _inputManager = new InputManager();
            EnableInputs(true);
            ListenInputManagerEvents(_inputManager);
        }

        private void ListenUnitEvents(Unit p_unit)
        {
            p_unit.TriggerDetector.onTriggerEnter = (Collider p_collider) =>
            {
                if (p_collider.tag == "Water")
                    IsImmerse = true;
            };  

            p_unit.TriggerDetector.onTriggerStay = (Collider p_collider) =>
            {
                if (p_collider.tag == "Water")
                    IsImmerse = true;
            };

            p_unit.TriggerDetector.onTriggerExit = (Collider p_collider) =>
            {
                if (p_collider.tag == "Water")
                    IsImmerse = false;
            };
        }

        private void ListenInteractionMenuEvents(InteractionMenu p_interactionMenu)
        {
            p_interactionMenu.onClickAction += InteractionMenu_OnClickAction;
        }

        private void InteractionMenu_OnClickAction(object p_source, OnClickActionEventArgs p_args)
        {
            switch(p_args.ObjectAction)
            {
                case ItemAction.Interact:
                    if (_isRightClickMoving)
                    {
                        _commandQuery.ClearQuery();
                        _isRightClickMoving = false;
                    }

                    AudioController.Instance.Play(Tags.SFX_Interact_Player);
                    _animator.SetBool("isWalking", true);
                    _commandQuery.AddCommand(new MoveCommand(_unit, p_args.MapItem.transform.position, (object p_commandSource, CommandCallbackEventArgs p_commandArgs) =>
                    {
                        _animator.SetBool("isWalking", false);
                        p_args.MapItem.Interact();
                    }));
                    break;
                case ItemAction.Cancel:
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
            if (p_inputInfo.phase == GesturePhaseType.START)
            {
                if (_interactionMenu.IsOpen == false)
                {
                    MapItem __mapItem = p_inputInfo.hit.GetComponent<MapItem>();
                    if (__mapItem != null)
                    {
                        AudioController.Instance.Play(Tags.SFX_Mouse_Click);
                        _interactionMenu.Open(__mapItem);
                    }
                }
            }
        }

        private void InputManager_OnMouseRightClick(InputInfo p_inputInfo)
        {
            if (p_inputInfo.phase == GesturePhaseType.START)
            {
                AudioController.Instance.Play(Tags.SFX_Interact_Player);

                if (_interactionMenu.IsOpen) _interactionMenu.Close();
                _isRightClickMoving = true;
                _animator.SetBool("isWalking", true);
                _commandQuery.ClearQuery();
                _commandQuery.AddCommand(new MoveCommand(_unit, p_inputInfo.worldClickPoint, (p_source, p_eventArgs) =>
                {
                    _animator.SetBool("isWalking", false);
                    _isRightClickMoving = false;
                }));
            }
        }

        private void ResetCameraPosition(bool p_immediatly = false)
        {
            if (p_immediatly)
                _mainCamera.transform.position = new Vector3(_unit.transform.position.x, _mainCamera.transform.position.y, _unit.transform.position.z - 16f);
            else
                _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, new Vector3(_unit.transform.position.x, _mainCamera.transform.position.y, _unit.transform.position.z - 16f), Time.deltaTime * 2f);
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

        private void CheckInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onPause?.Invoke();
            }

            if (Input.GetKey(KeyCode.Space))
            {
                ResetCameraPosition();
            }
        }
        #endregion
    }
}
