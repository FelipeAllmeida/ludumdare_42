using UnityEngine;

namespace Main.Settings
{
    public class GameSettingsSource : MonoBehaviour
    {
        public static GameSettingsSource Instance { get; private set; }

        public float menuAnimationDuration = 0.25f;
        public float groundSize = 10f;
        public float groundSize2 = 5f;
        public float wallWidth = 10f;
        public float wallSize = 1f;
        public float wallSize2 = .5f;
        public float waterMinHeight = .5f;
        public float waterMaxHeight = 4.25f;
        public float uiBarSize = 300f;
        public float forceFloodAdjacentTime = 10f;
        public float floodVelocity = 0.05f;
        public float waterPumpDecreaseFloodVelocity = 0.015f;
        public float waterPumpCost = 0.05f;
        public float mainFrameEnergyGain = 0.10f;

        private void Awake()
        {
            Instance = this;
        }
    }
}
