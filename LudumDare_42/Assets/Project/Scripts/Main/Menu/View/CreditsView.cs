using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Menu
{
    public class CreditsView : View
    {
        [Header("Buttons")]
        [SerializeField] private Button _buttonBack;

        [Header("Rect Transforms")]
        [SerializeField] private RectTransform _rectTransformButtonBack;
        [SerializeField] private RectTransform _rectTransformLayoutGroup;

        public override void Initialize()
        {
            _buttonBack.onClick.RemoveAllListeners();

            _buttonBack.onClick.AddListener(() => ChangeView(Views.Main));
        }

        protected override void EnableInputs(bool p_value)
        {
            _buttonBack.interactable = p_value;

            base.EnableInputs(p_value);
        }

        protected override void Animate(float p_value)
        {
            _rectTransformButtonBack.anchoredPosition = new Vector2(25f - (50f * (1f - p_value)), 0f);

            _rectTransformLayoutGroup.anchoredPosition = new Vector2(_rectTransformLayoutGroup.anchoredPosition.x,
                (-60f -_rectTransformLayoutGroup.rect.height / 2f) - (_rectTransformLayoutGroup.rect.height * (1f - p_value)));
        }
    }
}
