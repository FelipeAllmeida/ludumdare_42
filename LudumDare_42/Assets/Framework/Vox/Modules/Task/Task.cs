#if NETFX_CORE
using System;
using VoxInternal;

namespace Vox
{
    public class Task
    {
        public static TaskNode StartTask(Action p_callbackMethod, Action p_callbackFinish)
        {
            return StartTask("", p_callbackMethod, p_callbackFinish);
        }

        public static TaskNode StartTask(string p_tag, Action p_callbackMethod, Action p_callbackFinish)
        {
            TaskNode __node = new TaskNode(TaskModule.instance.GetNextNodeID(), p_tag, p_callbackMethod, p_callbackFinish);

            TaskModule.instance.AddNode(__node);

            return __node;
        }
    }
}
#endif