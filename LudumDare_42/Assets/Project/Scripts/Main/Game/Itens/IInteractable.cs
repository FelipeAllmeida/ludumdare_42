using Main.Game.Itens;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Main.Game.Itens
{
    public enum ItemAction
    {
        [Description("Interact")] Interact,
        [Description("Cancel")] Cancel,
    }

    public class OnChangeStateEventArgs : EventArgs
    {
        public OnChangeStateEventArgs(ItemState p_state) { State = p_state; }

        public ItemState State { get; private set; }
    }

    public interface IInteractable
    {
        event EventHandler<OnChangeStateEventArgs> onChangeState; 
        string ItemName { get; }
        ItemState State { get; }
        List<ItemAction> ListAction { get; }

        void Interact();
        void SetState(ItemState p_itemState);
    }
}
