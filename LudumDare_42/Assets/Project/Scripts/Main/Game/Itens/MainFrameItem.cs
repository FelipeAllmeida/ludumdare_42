using Internal.Audio;

namespace Main.Game.Itens
{
    public class MainFrameItem :  MapItem
    {
        public override void Interact()
        {
            if (State == ItemState.Disabled)
            {
                if (GameScore.CurrentEnergy - GameSettings.MAIN_FRAME_ENERGY_GAIN < 1f)
                {
                    base.Interact();
                    GameScore.AddEnergy(GameSettings.MAIN_FRAME_ENERGY_GAIN);
                    AudioController.Instance.Play(Tags.SFX_Interact_MainFrame);
                }
            }
        }
    }
}
