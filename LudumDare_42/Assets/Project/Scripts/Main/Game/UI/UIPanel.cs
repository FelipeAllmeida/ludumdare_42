using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Game.UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        public virtual void Initialize() { }

        public virtual void Enable(bool p_value, params object[] p_args)
        {
            gameObject.SetActive(p_value);
        }
    }
}
