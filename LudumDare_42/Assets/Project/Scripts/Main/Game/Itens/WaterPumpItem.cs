using Internal.Audio;
using UnityEngine;

namespace Main.Game.Itens
{
    public class WaterPumpItem : MapItem
    {
        [SerializeField] private Ground _ground;

        public override void Interact()
        {
            if (State == ItemState.Disabled)
            {
                if (GameScore.CurrentEnergy >= GameSettings.WATER_PUMP_COST)
                {
                    base.Interact();
                    GameScore.AddEnergy(-GameSettings.WATER_PUMP_COST);
                    AudioController.Instance.Play(Tags.SFX_Interact_WaterPump);
                    _ground.FloodVelocity -= GameSettings.WATER_PUMP_DECREASE_FLOOD_VELOCITY;
                }
            }
        }
    }
}
