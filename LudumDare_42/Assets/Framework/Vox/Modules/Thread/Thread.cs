using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxInternal;

namespace Vox
{
    public class Thread
    {
        public static ThreadNode StartThread(Action p_callbackMethod, Action p_callbackFinish)
        {
            return StartThread("", p_callbackMethod, p_callbackFinish);
        }

        public static ThreadNode StartThread(string p_tag, Action p_callbackMethod, Action p_callbackFinish)
        {
            ThreadNode __node = new ThreadNode(ThreadModule.instance.GetNextNodeID(), p_tag, p_callbackMethod, p_callbackFinish);

            ThreadModule.instance.AddNode(__node);

            return __node;
        }
    }
}
