using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonSettings : MonoBehaviour
{
    [SerializeField] private Text _buttonText;

    [SerializeField] private Button _buttonCore;


    public string Text
    {
        get { return _buttonText.text; }
        set { _buttonText.text = value; }
    }

    public Action ClickAction
    {
        set
        {
            _buttonCore.onClick.RemoveAllListeners();
            _buttonCore.onClick.AddListener(value.Invoke);
        }
    }
}
