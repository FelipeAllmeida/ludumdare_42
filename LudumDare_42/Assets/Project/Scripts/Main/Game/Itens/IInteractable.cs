using Main.Game.Itens;
using System.Collections.Generic;
using System.ComponentModel;

namespace Main.Game.Itens
{
    public enum ItemAction
    {
        [Description("Interact")] Interact,
        [Description("Cancel")] Cancel,
    }

    public interface IInteractable
    {
        string ItemName { get; }
        ItemState State { get; }
        List<ItemAction> ListAction { get; }

        void Interact();
        void SetState(ItemState p_itemState);
    }
}
