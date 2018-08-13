using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Menu
{
    public class TutorialView : View
    {
        [Header("Buttons")]
        [SerializeField] private UIButton _buttonBack;

        [Header("Rect Transforms")]
        [SerializeField] private RectTransform _rectTransformButtonBack;
        [SerializeField] private RectTransform _rectTransformLayoutGroup;

        public override void Initialize()
        {
            _buttonBack.Initialize(() => ChangeView(Views.Main));
        }

        protected override void EnableInputs(bool p_value)
        {
            base.EnableInputs(p_value);
            _buttonBack.interactable = p_value;
        }

        protected override void Animate(float p_value)
        {
            _rectTransformButtonBack.anchoredPosition = new Vector2(25f - (50f * (1f - p_value)), 0f);

            _rectTransformLayoutGroup.anchoredPosition = new Vector2(
                (_rectTransformLayoutGroup.rect.width - 321f) + (- _rectTransformLayoutGroup.rect.width * (1f - p_value)),
                _rectTransformLayoutGroup.anchoredPosition.y);
        }
    }
}
