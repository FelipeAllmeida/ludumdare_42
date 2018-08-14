using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Game.UI
{
    public enum GameOverType
    {
        Victory,
        Lost
    }

    public enum GameOverOptions
    {
        Menu,
        Exit
    }

    public class GameOverPanel : UIPanel
    {
        public event Action<GameOverOptions> onClick;

        [SerializeField] private Text _textHeaderGameOver;
        [SerializeField] private Text _textBodyGameOver;
        [SerializeField] private UIButton _buttonGoBackToMenu;
        [SerializeField] private UIButton _buttonExit;

        public override void Initialize()
        {
            base.Initialize();
            _buttonGoBackToMenu.Initialize(() => onClick?.Invoke(GameOverOptions.Menu));
            _buttonExit.Initialize(() => onClick?.Invoke(GameOverOptions.Exit));
        }

        public override void Enable(bool p_value, params object[] p_args)
        {
            if (p_value)
            {
                switch ((GameOverType)p_args[0])
                {
                    case GameOverType.Victory:
                        _textHeaderGameOver.text = "Victory!";
                        _textBodyGameOver.text = "Congratulations, you were able to survive long enough to be rescued!";
                        break;
                    case GameOverType.Lost:
                        _textHeaderGameOver.text = "Game Over :/";
                        _textBodyGameOver.text = "You weren't able to survive long enough to be rescued.";
                        break;
                    default:
                        break;
                }
            }

            base.Enable(p_value);
        }
    }
}
