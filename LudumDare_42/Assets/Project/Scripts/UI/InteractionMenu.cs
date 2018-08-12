using Main.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Vox;

namespace Main
{
    public class OnClickActionEventArgs : EventArgs
    {
        public OnClickActionEventArgs(ObjectAction p_objectAction, MapObject p_mapObject)
        { ObjectAction = p_objectAction;  MapObject = p_mapObject; }

        public ObjectAction ObjectAction { get; private set; }
        public MapObject MapObject { get; private set; }
    }

    public class InteractionMenu : MonoBehaviour
    {
        public event EventHandler<OnClickActionEventArgs> onClickAction;

        [SerializeField] private Camera _view;

        [SerializeField] private RectTransform _menuCanvas;
        [SerializeField] private RectTransform _menuPanel;

        [SerializeField] private Button _menuButton;

        private TimerNode _nodeClose;

        public bool IsOpen { get; private set; }

        public void SetCamera(Camera p_camera)
        {
            _view = p_camera;
            _menuCanvas.GetComponent<Canvas>().worldCamera = p_camera;
        }

        public void Close()
        {
            ActivateContextMenu(false);
        }

        public void Open(MapObject p_object)
        {
            _nodeClose?.Cancel();

            foreach (GameObject __child in _menuPanel.GetComponentsInChildren<RectTransform>().Select(c => c.gameObject).Except(new[] { _menuPanel.gameObject }))
            {
                Destroy(__child);
            }

            _menuPanel.DetachChildren();

            Vector2 __point;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_menuCanvas, Input.mousePosition, _view, out __point);

            Debug.Log($"Point: {__point}");

            IActor __actions = p_object as IActor;

            Vector2 __buttonSize = _menuButton.GetComponent<RectTransform>().sizeDelta;

            _menuPanel.sizeDelta = new Vector2(_menuPanel.sizeDelta.x, __buttonSize.y * __actions.ActionsList.Count);
            _menuPanel.position = _menuCanvas.TransformPoint(__point);

            Debug.Log($"_menuPanel.position: {_menuPanel.position}");

            __actions.ActionsList.Select(p_objectAction =>
            {
                RectTransform __actionButton = Instantiate(_menuButton).GetComponent<RectTransform>();

                ButtonSettings __settings = __actionButton.GetComponent<ButtonSettings>();

                __settings.Text = p_objectAction.GetDescription();
                __settings.ClickAction = () =>
                {
                    ActivateContextMenu(false);
                    onClickAction?.Invoke(null, new OnClickActionEventArgs(p_objectAction, p_object));
                };

                return __actionButton;
            }).ToList().ForEach(b =>
            {
                b.SetParent(_menuPanel);
                b.localScale = Vector3.one;
                b.localRotation = Quaternion.identity;
                b.localPosition = Vector3.zero;

                b.offsetMax = Vector2.down * 12 * (_menuPanel.childCount - 1);
                b.offsetMin = Vector2.down * 12 * _menuPanel.childCount;
            });

            ActivateContextMenu(true);

            _nodeClose = Timer.WaitSeconds(2f, () => ActivateContextMenu(false));
        }

        private void ActivateContextMenu(bool p_value)
        {
            IsOpen = p_value;
            _menuPanel.gameObject.SetActive(p_value);
        }
    }

}
