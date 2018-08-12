using Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internal
{

    public enum CommandType
    {
        NONE,
        MOVE,
        GATHER,
        ATTACK,
        BUILD,
        STOP,
        RANGED_ATTACK
    }

    public class CommandController
    {
        #region Events
        public Action onCurrentCommandFinish;
        #endregion

        private CommandType _currentCommand;

        private Command _command;

        public void AInitialize()
        {
        }

        public void UpdateCommands()
        {
            if (_command != null)
            {
                _command.AUpdate();
            }
        }

        public void MoveTo(Unit p_actor, Vector3 p_targetPosition)
        {
            _command = new MoveCommand(p_actor, p_targetPosition, onCurrentCommandFinish, ClearCurrentCommand);
            _currentCommand = _command.Execute();
        }

        public void UpdateMoveDestination(Vector3 p_targetPosition)
        {
            if (_command != null && _command is MoveCommand)
            {
                (_command as MoveCommand).SetDestination(p_targetPosition);
            }
            else
            {
                if (_command != null)
                {
                    Debug.LogWarning(string.Format("{0} is not a Move Command", _currentCommand));
                }
            }
        }

        public CommandType GetCurrentCommand()
        {
            return _currentCommand;
        }

        public void StopCurrentCommand()
        {
            if (_command != null)
            {
                _command.Stop();
            }

            ClearCurrentCommand();
        }

        private void ClearCurrentCommand()
        {
            onCurrentCommandFinish = null;

            _command = null;

            _currentCommand = CommandType.NONE;
        }
    }

}
