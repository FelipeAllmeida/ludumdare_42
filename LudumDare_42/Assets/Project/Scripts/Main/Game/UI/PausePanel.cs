using System;
using UnityEngine;

namespace Main.Game.UI
{
    public enum PauseOptions
    {
        Resume,
        Menu,
        Exit
    }

    public class PausePanel : UIPanel
    {
        public event Action<PauseOptions> onClick;

        [SerializeField] private UIButton _buttonResume;
        [SerializeField] private UIButton _buttonMenu;
        [SerializeField] private UIButton _buttonExit;

        public override void Initialize()
        {
            base.Initialize();
            _buttonResume.Initialize(() => onClick?.Invoke(PauseOptions.Resume));
            _buttonMenu.Initialize(() => onClick?.Invoke(PauseOptions.Menu));
            _buttonExit.Initialize(() => onClick?.Invoke(PauseOptions.Exit));
        }
    }
}
