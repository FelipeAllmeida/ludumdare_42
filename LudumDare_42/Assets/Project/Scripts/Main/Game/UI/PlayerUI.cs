using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Game.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private UIBarObject _energyBar;
        [SerializeField] private UIBarObject _oxygenBar;
        [SerializeField] private UIClockObject _clockObject;

        public UIBarObject EnergyBar { get { return _energyBar; } }
        public UIBarObject OxygenBar { get { return _oxygenBar; } }
        public UIClockObject Clock { get { return _clockObject; } }
    }
}
