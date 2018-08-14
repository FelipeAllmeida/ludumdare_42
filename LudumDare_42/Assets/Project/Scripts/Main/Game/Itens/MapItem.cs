using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Game.Itens
{
    public enum ItemState
    {
        Enabled,
        Disabled
    }

    public abstract class MapItem : MonoBehaviour, IInteractable
    {
        private List<ItemAction> _listActions;
        public List<ItemAction> ListAction => _listActions ?? (_listActions = new List<ItemAction> { ItemAction.Interact, ItemAction.Cancel });

        [SerializeField] private ItemState _state = ItemState.Disabled;
        public ItemState State { get { return _state; } }

        [SerializeField] private string _itemName;

        public event EventHandler<OnChangeStateEventArgs> onChangeState;

        public string ItemName { get { return _itemName; } }

        public virtual void Interact()
        {
            _state = (State == ItemState.Disabled) ? ItemState.Enabled : ItemState.Disabled;
            onChangeState?.Invoke(this, new OnChangeStateEventArgs(_state));
        }

        public virtual void SetState(ItemState p_state)
        {
            _state = p_state;
        }
    }
}
