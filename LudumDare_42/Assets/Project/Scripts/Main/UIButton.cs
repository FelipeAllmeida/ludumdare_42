using Internal.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    public class UIButton : MonoBehaviour, IButton
    {
        public event Action onClick;

        [SerializeField] private Button _button;

        public bool interactable
        {
            get
            {
                return _button.interactable;
            }

            set
            {
                _button.interactable = value;
            }
        }

        public void Initialize(Action p_action)
        {
            onClick = null;
            onClick += p_action;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(Click);
        }

        public void Click()
        {
            AudioController.Instance.Play(Tags.SFX_Mouse_Click_Button);
            onClick?.Invoke();
        }
    }
}
