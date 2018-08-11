using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class Ground : MapObject
    {
        public enum State
        {
            FLOOD_SOURCE,
            DRY,
            FLOODED,
            IMMERSE
        }

        #region Events
        public class OnChangeStateEventArgs : EventArgs
        {
            public OnChangeStateEventArgs(State p_state, float p_fillAmount) { State = p_state; FillAmount = p_fillAmount; }
            public State State { get; private set; }
            public float FillAmount { get; private set; }
        }

        public event EventHandler<OnChangeStateEventArgs> onChangeState;
        #endregion

        #region Public Data
        public bool isFloodSource;
        public State CurrentState { get; private set; }
        #endregion

        #region Private Data
        [Range(0f, 1f)]
        [SerializeField] private float _startFillAmount;
        [SerializeField] private Water _water;

        private float _currentFillAmount;

        private Vector3 _position;
        #endregion

        #region Methods
        public override void Initialize(int p_x, int p_y)
        {
            base.Initialize(p_x, p_y);

            if (isFloodSource) CurrentState = State.FLOOD_SOURCE;
        }

        public void UpdateFillAmount(float p_floodVelocity)
        {
            switch(CurrentState)
            {
                case State.FLOOD_SOURCE:
                case State.FLOODED:
                    SetFillAmount(_currentFillAmount + (Time.deltaTime * p_floodVelocity));
                    break;
                case State.IMMERSE:
                case State.DRY:
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

            CurrentState = p_state;

            _water.SetActive(CurrentState != State.DRY);

            onChangeState?.Invoke(this, new OnChangeStateEventArgs(CurrentState, _currentFillAmount));
        }
        #endregion
    }
}
