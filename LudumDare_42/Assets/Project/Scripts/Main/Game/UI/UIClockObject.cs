using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Game.UI
{
    public class UIClockObject : MonoBehaviour
    {
        [SerializeField] private Text _text;

        public void SetClockTime(TimeSpan p_time)
        {
            _text.text = p_time.ToString(@"mm\:ss\:ff");
        }
    }
}
