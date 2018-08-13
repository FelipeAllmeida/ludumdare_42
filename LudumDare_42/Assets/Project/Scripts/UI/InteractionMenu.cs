using Main.Game.Itens;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vox;

namespace Main
{
    public class OnClickActionEventArgs : EventArgs
    {
        public OnClickActionEventArgs(ItemAction p_objectAction, MapItem p_mapObject)
        { ObjectAction = p_objectAction;  MapItem = p_mapObject; }

        public ItemAction ObjectAction { get; private set; }
        public MapItem MapItem { get; private set; }
    }

    public class InteractionMenu : MonoBehaviour
    {
        public event EventHandler<OnClickActionEventArgs> onClickAction;

        [SerializeField] private Camera _view;

        [SerializeField] private RectTransform _menuCanvas;
        [SerializeField] private RectTransform _menuPanel;
        [SerializeField] private Text _menuText;

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

        public void Open(MapItem p_object)
        {
            _nodeClose?.Cancel();

            foreach (GameObject __child in _menuPanel.GetComponentsInChildren<Button>().Select(c => c.gameObject))
            {
                Destroy(__child);
            }

            Vector2 __point;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_menuCanvas, Input.mousePosition, _view, out __point);

            IInteractable __actions = p_object as IInteractable;

            Vector2 __buttonSize = _menuButton.GetComponent<RectTransform>().sizeDelta;

            _menuPanel.sizeDelta = new Vector2(_menuPanel.sizeDelta.x, 26f + __buttonSize.y * __actions.ListAction.Count + 3 * __actions.ListAction.Count);
            _menuPanel.position = _menuCanvas.TransformPoint(__point);
            _menuText.text = p_object.ItemName;
            __actions.ListAction.Select(p_objectAction =>
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

                b.sizeDelta = new Vector2(90f, 15f);
            });

            ActivateContextMenu(true);

            _nodeClose = Timer.WaitSeconds(2f, () => ActivateContextMenu(false));
        }

        private void ActivateContextMenu(bool p_value)
        {
            _nodeClose?.Cancel();
            IsOpen = p_value;
            _menuPanel.gameObject.SetActive(p_value);
        }
    }

}
