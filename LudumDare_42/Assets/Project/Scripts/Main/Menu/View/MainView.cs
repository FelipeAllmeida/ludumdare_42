using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Menu
{
    public class MainView : View
    {
        public event Action onClickButtonStart;

        [Header("References")]
        [SerializeField] private Button _buttonStart;
        [SerializeField] private Button _buttonCredits;
        [SerializeField] private Button _buttonExit;
        [Header("Rect Transforms")]
        [SerializeField] private RectTransform _rectTransformLeftPanel;
        [SerializeField] private RectTransform _rectTransformRightPanel;

        public override void Initialize()
        {
            _buttonStart.onClick.RemoveAllListeners();
            _buttonCredits.onClick.RemoveAllListeners();
            _buttonExit.onClick.RemoveAllListeners();

            _buttonStart.onClick.AddListener(() => onClickButtonStart?.Invoke());
            _buttonCredits.onClick.AddListener(() => ChangeView(Views.Credits));
            _buttonExit.onClick.AddListener(() => Application.Quit());
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
