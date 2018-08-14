using Main.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Game.UI
{

    public class PlayerUI : MonoBehaviour
    {
        public event Action<GameOverOptions> onGameOverFinish;
        public event Action<PauseOptions> onPauseFinish;

        [Header("References")]

        [SerializeField] private GameOverPanel _gameOverPanel;
        [SerializeField] private PausePanel _pausePanel;

        [SerializeField] private UIBarObject _energyBar;
        [SerializeField] private UIBarObject _oxygenBar;
        [SerializeField] private UIClockObject _clockObject;

        public UIBarObject EnergyBar { get { return _energyBar; } }
        public UIBarObject OxygenBar { get { return _oxygenBar; } }
        public UIClockObject Clock { get { return _clockObject; } }

        public void Initialize()
        {
            _gameOverPanel.Initialize();
            _pausePanel.Initialize();

            ListenEvents();
        }

        public void EnableGameOverPanel(GameOverType p_gameOver)
        {
            _gameOverPanel.Enable(true, p_gameOver);
        }

        public void EnablePausePanel(bool p_value)
        {
            _pausePanel.Enable(p_value);
        }

        public void DisableGameOverPanel()
        {
            _gameOverPanel.Enable(false);
        }

        private void ListenEvents()
        {
            _gameOverPanel.onClick -= GameOverPanel_OnClick;
            _gameOverPanel.onClick += GameOverPanel_OnClick;

            _pausePanel.onClick -= PausePanel_OnClick;
            _pausePanel.onClick += PausePanel_OnClick;
        }

        private void GameOverPanel_OnClick(GameOverOptions p_options)
        {
            onGameOverFinish?.Invoke(p_options);
        }

        private void PausePanel_OnClick(PauseOptions p_options)
        {
            onPauseFinish?.Invoke(p_options);
        }
    }
}
