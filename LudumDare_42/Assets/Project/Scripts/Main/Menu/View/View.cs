using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vox;

namespace Main.Menu
{
    public interface IView
    {
        void Initialize();
        void Show(Action p_callbackFinish);
        void Hide(Action p_callbackFinish);
        void ForceHide();
    }

    public abstract class View : MonoBehaviour, IView
    {
        public enum ViewStates
        {
            Showing,
            Hided
        }

        public enum ViewAnimations
        {
            Show,
            Hide
        }

        [SerializeField] private Views _menuType;
        public Views Type {get {return _menuType;} }

        public ViewStates State { get; private set; } = ViewStates.Hided;

        public EventHandler<Views> onRequestChangeView;
        protected TweenNode _nodeAnimation;

        protected void ChangeView(Views p_nextMenu)
        {
            onRequestChangeView?.Invoke(this, p_nextMenu);
        }

        public virtual void Initialize()
        {
        }

        public void Hide(Action p_action)
        {
            EnableInputs(false);
            StartAnimation(ViewAnimations.Hide, () =>
            {
                State = ViewStates.Hided;
                gameObject.SetActive(false);
                p_action?.Invoke();
            });
        }

        public void ForceHide()
        {
            State = ViewStates.Hided;
            gameObject.SetActive(false);
        }

        public void Show(Action p_action)
        {
            EnableInputs(false);
            gameObject.SetActive(true);
            State = ViewStates.Showing;
            StartAnimation(ViewAnimations.Show, () =>
            {
                EnableInputs(true);
                p_action?.Invoke();
            });
        }

        protected virtual void EnableInputs(bool p_value)
        {

        }

        protected void StartAnimation(ViewAnimations p_animation, Action p_action)
        {
            float __startValue, __endValue;
            switch (p_animation)
            {
                case ViewAnimations.Show:
                    __startValue = 0f;
                    __endValue = 1f;
                    break;
                case ViewAnimations.Hide:
                    __startValue = 1f;
                    __endValue = 0f;
                    break;
                default:
                    throw new NotImplementedException($"Animation {p_animation} not implemented.");
            }

            _nodeAnimation?.Cancel();
            _nodeAnimation = Tween.FloatTo(__startValue, __endValue, GameSettings.MENU_ANIMATION_DURATION, EaseType.LINEAR, Animate);
            _nodeAnimation.onFinish = p_action;
        }

        protected virtual void Animate(float p_value)
        {

        }
    }
}
