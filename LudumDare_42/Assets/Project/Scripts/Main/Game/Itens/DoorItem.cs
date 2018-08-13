using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vox;

namespace Main.Game.Itens
{
    public class DoorItem : MapItem
    {
        private TweenNode _nodeOpenAnimation;

        public override void Interact()
        {
            base.Interact();
            StartAnimation(State);
        }

        public override void SetState(ItemState p_state)
        {
            base.SetState(p_state);
            transform.localScale = new Vector3(1f, (State == ItemState.Disabled) ? 0.2f : 1f, 1f);
        }

        private void StartAnimation(ItemState p_state)
        {
            float __start = (p_state == ItemState.Disabled) ? 1f : 0.2f;
            float __finish = (p_state == ItemState.Disabled) ? 0.2f : 1f;

            _nodeOpenAnimation?.Cancel();
            _nodeOpenAnimation = Tween.FloatTo(__start, __finish, .25f, EaseType.LINEAR, (float p_value) =>
            {
                transform.localScale = new Vector3(1f, p_value, 1f);
            });
        }
    }
}
