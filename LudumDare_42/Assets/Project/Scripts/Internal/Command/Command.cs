using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internal.Commands
{
    public class CommandCallbackEventArgs : EventArgs
    {
        public CommandCallbackEventArgs(Command p_command) { Command = p_command; }

        public Command Command { get; private set; }
    }

    [Serializable]
    public abstract class Command
    {
        public enum State
        {
            OnQuery,
            Running,
            Stopped,
            Finished
        }

        public Commands CommandType { get; private set; }
        public State CurrentState { get; private set; }

        public event EventHandler<CommandCallbackEventArgs> CallbackFinish;
        public event EventHandler<CommandCallbackEventArgs> CallbackStopped;

        public Command(Commands p_commandType)
        {
            CurrentState = State.OnQuery;
            CommandType = p_commandType;
        }

        public virtual Commands Execute()
        {
            CurrentState = State.Running;
            return CommandType;
        }

        public virtual void Resume()
        {

        }

        public virtual void Pause()
        {

        }

        public virtual void AUpdate()
        {

        }

        public virtual void Stop()
        {
            CurrentState = State.Stopped;
            CallbackStopped?.Invoke(null, new CommandCallbackEventArgs(this));
        }

        public virtual void Undo()
        {

        }

        protected virtual void Finish()
        {
            CurrentState = State.Finished;
            CallbackFinish?.Invoke(null, new CommandCallbackEventArgs(this));
        }
    }
}
