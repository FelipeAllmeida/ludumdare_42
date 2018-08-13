using Main.Settings;

namespace Main
{
    public static class GameSettings
    {
        public static float MENU_ANIMATION_DURATION { get { return GameSettingsSource.Instance.menuAnimationDuration; } }
        public static float GROUND_SIZE { get { return GameSettingsSource.Instance.groundSize; } }
        public static float GROUND_SIZE2 { get { return GameSettingsSource.Instance.groundSize2; } }
        public static float WALL_WIDTH { get { return GameSettingsSource.Instance.wallWidth; } }
        public static float WALL_SIZE { get { return GameSettingsSource.Instance.wallSize; } }
        public static float WALL_SIZE2 { get { return GameSettingsSource.Instance.wallSize2; } }
        public static float WATER_MIN_HEIGHT { get { return GameSettingsSource.Instance.waterMinHeight; } }
        public static float WATER_MAX_HEIGHT { get { return GameSettingsSource.Instance.waterMaxHeight; } }
        public static float UI_BAR_SIZE_X { get { return GameSettingsSource.Instance.uiBarSize; } }
        public static float FORCE_FLOOD_ADJACENT_TIME { get { return GameSettingsSource.Instance.forceFloodAdjacentTime; } }
        public static float FLOOD_VELOCITY { get { return GameSettingsSource.Instance.floodVelocity; } }
        public static float WATER_PUMP_DECREASE_FLOOD_VELOCITY { get { return GameSettingsSource.Instance.waterPumpDecreaseFloodVelocity; } }
        public static float WATER_PUMP_COST { get { return GameSettingsSource.Instance.waterPumpCost; } }
        public static float MAIN_FRAME_ENERGY_GAIN { get { return GameSettingsSource.Instance.mainFrameEnergyGain; } }
    }
}
