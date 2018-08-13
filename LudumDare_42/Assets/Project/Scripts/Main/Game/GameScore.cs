using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Game
{
    public static class GameScore
    {
        public static float CurrentEnergy { get; private set; }
        public static float CurrentOxygen { get; private set; }

        public static void AddEnergy(float p_value)
        {
            CurrentEnergy = Mathf.Clamp01(CurrentEnergy + p_value);
        }

        public static void AddOxygen(float p_value)
        {
            CurrentOxygen = Mathf.Clamp01(CurrentOxygen + p_value);
        }
    }
}
