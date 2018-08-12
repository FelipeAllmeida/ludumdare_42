using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Vox;

public class Door : Wall, IActor
{
    public enum State
    {
        OPEN,
        CLOSED
    }

    #region Events
    public class OnChangeStateEventArgs : EventArgs
    {
        public OnChangeStateEventArgs(State p_state) { State = p_state; }
        public State State { get; private set; }
    }

    public event EventHandler<OnChangeStateEventArgs> onChangeState;
    #endregion

    [SerializeField] private State _currentState = State.OPEN;
    public State CurrentState { get { return _currentState; } }

    [SerializeField] private Transform _trasformPivotDoor;
    [SerializeField] private List<Transform> _listTransformsDoor;

    private TweenNode _nodeOpenAnimation;

    private List<ObjectAction> _actionsList;
    public List<ObjectAction> ActionsList => _actionsList ?? (_actionsList = new List<ObjectAction> { ObjectAction.Interact, ObjectAction.Cancel });

    public override void Interact()
    {
        if (CurrentState == State.OPEN)
            _currentState = State.CLOSED;
        else
            _currentState = State.OPEN;

        float __start = (CurrentState == State.OPEN) ? 1f : 0.2f;
        float __finish = (CurrentState == State.OPEN) ? 0.2f : 1f;

        _nodeOpenAnimation?.Cancel();
        _nodeOpenAnimation = Tween.FloatTo(__start, __finish, .25f, EaseType.LINEAR, (float p_value) =>
        {
            _trasformPivotDoor.localScale = new Vector3(1f, p_value, 1f);
        });

        onChangeState?.Invoke(this, new OnChangeStateEventArgs(CurrentState));
    }

    public override void SetPosition(Vector3 p_position)
    {
        base.SetPosition(p_position);

        if (IsHorizontal)
        {
            _listTransformsDoor.ForEach(x => 
            {
                switch(x.name)
                {
                    case "Wall_0":
                        x.transform.localPosition = new Vector3(-0.3333333f, 0f, 0f);
                        break;
                    case "Wall_1":
                        x.transform.localPosition = new Vector3(0.3333333f, 0f, 0f);
                        break;
                }
            });
        }
        else
        {
            _listTransformsDoor.ForEach(x => 
            {
                switch (x.name)
                {
                    case "Wall_0":
                        x.transform.localPosition = new Vector3(0f, 0f, - 0.3333333f);
                        break;
                    case "Wall_1":
                        x.transform.localPosition = new Vector3(0f, 0f, 0.3333333f);
                        break;
                }
            });
        }
    }

    public override void SetLocalScale(Vector3 p_localScale)
    {
        base.SetLocalScale(p_localScale);

        bool __isHorizontal = p_localScale.x > p_localScale.z;

        if (__isHorizontal)
        {
            _listTransformsDoor.ForEach(x => x.transform.localScale = new Vector3(0.3333333f, 1f, 1f));
        }
        else
        {
            _listTransformsDoor.ForEach(x => x.transform.localScale = new Vector3(1f, 1f, 0.3333333f));
        }
    }

}
