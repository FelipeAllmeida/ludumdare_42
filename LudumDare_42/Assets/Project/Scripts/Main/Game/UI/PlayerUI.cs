using Main.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Game.UI
{
    public enum GameOvers
    {
        Victory,
        Lost
    }
    public class PlayerUI : MonoBehaviour
    {
        public event Action onClickGoToMenu;

        [Header("References")]
        [SerializeField] private GameObject _objectGameOverPanel;
        [SerializeField] private Text _textHeaderGameOver;
        [SerializeField] private Text _textBodyGameOver;
        [SerializeField] private UIButton _buttonGoBackToMenu;
        [SerializeField] private UIButton _buttonExit;
        [SerializeField] private UIBarObject _energyBar;
        [SerializeField] private UIBarObject _oxygenBar;
        [SerializeField] private UIClockObject _clockObject;

        public UIBarObject EnergyBar { get { return _energyBar; } }
        public UIBarObject OxygenBar { get { return _oxygenBar; } }
        public UIClockObject Clock { get { return _clockObject; } }

        public void Initialize()
        {
            _buttonGoBackToMenu.Initialize(() =>
            {
                onClickGoToMenu?.Invoke();
            });
            _buttonExit.Initialize(() => Application.Quit());
        }

        public void EnableGameOverPanel(GameOvers p_gameOver)
        {
            switch(p_gameOver)
            {
                case GameOvers.Victory:
                    _textHeaderGameOver.text = "Victory!";
                    _textBodyGameOver.text = "Congratulations, you were able to survive long enough to be rescued!";
                    break;
                case GameOvers.Lost:
                    _textHeaderGameOver.text = "Game Over :/";
                    _textBodyGameOver.text = "You weren't able to survive long enough to be rescued.";
                    break;
            }
            _objectGameOverPanel.SetActive(true);
        }

        public void DisableGameOverPanel()
        {
            _objectGameOverPanel.SetActive(false);
        }
    }
}
