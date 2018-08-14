using Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Internal.Commands
{
    public enum Commands
    {
        NONE,
        STOP,
        MOVE
    }

    public class CommandQuery
    {
        public CommandQuery()
        {
            _listCommands = new List<Command>();
        }

        #region Events
        public Action onCurrentCommandFinish;
        #endregion

        [SerializeField] private List<Command> _listCommands;

        public void Resume()
        {
            if (_listCommands.Count > 0)
            {
                _listCommands[0]?.Resume();
            }
        }

        public void Pause()
        {
            if (_listCommands.Count > 0)
            {
                _listCommands[0]?.Pause();
            }
        }

        public void UpdateQuery()
        {
            if (_listCommands.Count > 0)
            {
                _listCommands[0]?.AUpdate();
            }
        }

        public void ClearQuery()
        {
            if (_listCommands.Count > 0)
            {
                _listCommands[0]?.Stop();
            }

            _listCommands.Clear();
        }

        public void AddCommand(Command p_command)
        {
            p_command.CallbackFinish += Command_Callback;
            p_command.CallbackStopped += Command_Callback;

            _listCommands.Add(p_command);

            if (_listCommands.Count == 1) RunNext();
        }

        public Commands GetCurrentCommand()
        {
            return _listCommands.Count > 0 ? _listCommands[0].CommandType : Commands.NONE;
        }

        #region Internal
        private void RunNext()
        {
            if (_listCommands.Count > 0 && _listCommands[0].CurrentState == Command.State.OnQuery)
            {
                _listCommands[0].Execute();
            }
        }

        private void RemoveCommand(Command p_command)
        {
            _listCommands.Remove(p_command);
        }

        private void Command_Callback(object p_source, CommandCallbackEventArgs p_args)
        {
            RemoveCommand(p_args.Command);

            if (p_args.Command.CurrentState == Command.State.Finished)
            {
                RunNext();
            }
        }
        #endregion
    }

}
