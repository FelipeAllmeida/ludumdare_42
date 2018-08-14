using Main.Game.Itens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Main.Game
{
    public class WallDoor : Wall
    {
        #region Events
        public class OnChangeStateEventArgs : EventArgs
        {
            public OnChangeStateEventArgs(ItemState p_state) { State = p_state; }
            public ItemState State { get; private set; }
        }

        public event EventHandler<OnChangeStateEventArgs> onChangeState;
        #endregion

        [Header("References")]
        [SerializeField] private DoorItem _doorItem;
        [SerializeField] private List<Transform> _listTransformsDoor;
        
        private List<ItemAction> _actionsList;
        public List<ItemAction> ActionsList => _actionsList ?? (_actionsList = new List<ItemAction> { ItemAction.Interact, ItemAction.Cancel });

        public ItemState CurrentState { get { return _doorItem.State; } }

        public override void Initialize(int p_x, int p_y)
        {
            base.Initialize(p_x, p_y);
            ListenEvents();
        }

        public override void ForceInteract()
        {
            _doorItem.Interact();
        }

        public void SetState(ItemState p_state)
        {
            _doorItem.SetState(p_state);
        }

        public override void Clear()
        {
            base.Clear();
            onChangeState = null;
        }

        public override void SetPosition(Vector3 p_position)
        {
            base.SetPosition(p_position);

            if (IsHorizontal)
            {
                _listTransformsDoor.ForEach(x =>
                {
                    switch (x.name)
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
                            x.transform.localPosition = new Vector3(0f, 0f, -0.3333333f);
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

        private void ListenEvents()
        {
            _doorItem.onChangeState -= DoorItem_OnChangeState;
            _doorItem.onChangeState += DoorItem_OnChangeState;
        }

        private void DoorItem_OnChangeState(object p_source, Itens.OnChangeStateEventArgs p_eventArgs)
        {
            onChangeState?.Invoke(this, new OnChangeStateEventArgs(p_eventArgs.State));
        }

    }
}