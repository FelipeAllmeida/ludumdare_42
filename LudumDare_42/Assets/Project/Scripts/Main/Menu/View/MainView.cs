using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Menu
{
    public class MainView : View
    {
        public event Action onClickButtonStart;

        [Header("References")]
        [SerializeField] private UIButton _buttonStart;
        [SerializeField] private UIButton _buttonTutorial;
        [SerializeField] private UIButton _buttonCredits;
        [SerializeField] private UIButton _buttonExit;
        [Header("Rect Transforms")]
        [SerializeField] private RectTransform _rectTransformLeftPanel;
        [SerializeField] private RectTransform _rectTransformRightPanel;

        public override void Initialize()
        {
            _buttonStart.Initialize(() => onClickButtonStart?.Invoke());
            _buttonTutorial.Initialize(() => ChangeView(Views.Tutorial));
            _buttonCredits.Initialize(() => ChangeView(Views.Credits));
            _buttonExit.Initialize(() => Application.Quit());
        }

        protected override void Animate(float p_value)
        {
            _rectTransformLeftPanel.anchoredPosition = new Vector2((-_rectTransformLeftPanel.rect.width / 2f) - (_rectTransformLeftPanel.rect.width * (1f - p_value)),
                _rectTransformLeftPanel.anchoredPosition.y);

            _rectTransformRightPanel.anchoredPosition = new Vector2((_rectTransformRightPanel.rect.width / 2f) + (_rectTransformRightPanel.rect.width * (1f - p_value)),
               _rectTransformRightPanel.anchoredPosition.y);
        }

        protected override void EnableInputs(bool p_value)
        {
            _buttonStart.interactable = p_value;
            _buttonCredits.interactable = p_value;
            _buttonExit.interactable = p_value;

            base.EnableInputs(p_value);
        }
    }
}
