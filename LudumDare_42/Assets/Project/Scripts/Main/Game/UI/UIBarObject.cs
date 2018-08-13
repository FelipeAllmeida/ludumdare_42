using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Game.UI
{
    public class UIBarObject : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private Image _imageBar;

        public void SetFillProgress(float p_value)
        {
            _imageBar.rectTransform.sizeDelta = new Vector2(GameSettings.UI_BAR_SIZE_X * Mathf.Clamp01(p_value), _imageBar.rectTransform.sizeDelta.y);
        }

        public void ResetBar()
        {
            SetFillProgress(1f);
        }

    }
}