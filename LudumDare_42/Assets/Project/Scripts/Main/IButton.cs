using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public interface IButton
    {
        event Action onClick;

        bool interactable { get; set; }

        void Initialize(Action action);
        void Click();
    }
}
