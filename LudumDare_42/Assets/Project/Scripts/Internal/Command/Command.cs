using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Internal
{
    public abstract class Command 
    {
        protected Action onCommandFinish;
        protected Action onCommandStopped;

        public virtual CommandType Execute()
        {
            return CommandType.NONE;
        }

        public virtual void AUpdate()
        {

        }

        public virtual void Stop()
        {
            onCommandStopped?.Invoke();
        }

        public virtual void Undo()
        {
      
        }
    }
}
