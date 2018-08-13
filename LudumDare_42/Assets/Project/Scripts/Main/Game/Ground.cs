using Internal.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vox;

namespace Main.Game
{
    public class Ground : MapObject
    {
        public enum State
        {
            DRY,
            FLOODED,
            IMMERSE,
            FLOOD_SOURCE
        }

        #region Events
        public class OnChangeStateEventArgs : EventArgs
        {
            public OnChangeStateEventArgs(State p_state, float p_fillAmount) { State = p_state; FillAmount = p_fillAmount; }
            public State State { get; private set; }
            public float FillAmount { get; private set; }
        }

        public event EventHandler<OnChangeStateEventArgs> onChangeState;
        public event EventHandler onMaxPressure;
        #endregion

        #region Public Data
        public bool isFloodSource;
        [SerializeField] private State _currentState;
        public State CurrentState { get { return _currentState; } }
        #endregion

        #region Private Data
        [Range(0f, 1f)]
        [SerializeField] private float _startFillAmount;
        [SerializeField] private Water _water;

        private NavMeshSourceTag _navMeshSourceTag;
        private float _currentFillAmount;
        private TimerNode _timerNode;

        private Vector3 _position;
        #endregion

        #region Methods
        public override void Initialize(int p_x, int p_y)
        {
            base.Initialize(p_x, p_y);
            _navMeshSourceTag = transform.GetChild(2).GetComponent<NavMeshSourceTag>();
            if (isFloodSource) SetState(State.FLOOD_SOURCE);
        }

        public void UpdateFillAmount(float p_floodVelocity)
        {
            switch(CurrentState)
            {
                case State.FLOOD_SOURCE:
                case State.FLOODED:
                    SetFillAmount(_currentFillAmount + (Time.deltaTime * p_floodVelocity));
                    break;
                case State.DRY:
                case State.IMMERSE:
                    break;
            }
        }

        public void SetFillAmount(float p_fillAmount)
        {
            _currentFillAmount = Mathf.Clamp01(p_fillAmount);

            if (CurrentState != State.FLOOD_SOURCE)
            {
                if (p_fillAmount >= 1) { SetState(State.IMMERSE); }
                else if (p_fillAmount > 0 && p_fillAmount < 1) { SetState(State.FLOODED); }
                else { SetState(State.DRY); }
            }
            else
            {
                if (p_fillAmount >= 1) { SetState(State.IMMERSE); }
            }

            switch (CurrentState)
            {
                case State.DRY:
                    break;
                case State.FLOODED:
                case State.IMMERSE:
                case State.FLOOD_SOURCE:
                    _water.SetFillAmount(_currentFillAmount);
                    break;
            }
        }

        #endregion

        #region Internal
        private void SetState(State p_state)
        {
            if (CurrentState == p_state) return;


            _currentState = p_state;

            _water.SetActive(CurrentState != State.DRY);
            _navMeshSourceTag.enabled = CurrentState != State.IMMERSE;

            switch(CurrentState)
            {
                case State.FLOODED:
                    AudioController.Instance.Play(Tags.Ambience_WaterFillingAdjacent);
                    break;
                case State.IMMERSE:
                    _timerNode?.Cancel();
                    _timerNode = Timer.WaitSeconds(GameSettings.FORCE_FLOOD_ADJACENT_TIME, () =>
                    {
                        onMaxPressure?.Invoke(this, null);
                    });
                    break;
            }

            onChangeState?.Invoke(this, new OnChangeStateEventArgs(CurrentState, _currentFillAmount));
        }
        #endregion
    }
}
