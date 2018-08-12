using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using Image = UnityEngine.UI.Image;

public class InteractionMenu : MonoBehaviour
{
    [SerializeField] private Camera _view;

    [SerializeField] private RectTransform _menuCanvas;
    [SerializeField] private RectTransform _menuPanel;

    [SerializeField] private Button _menuButton;

	void Update ()
	{
	    Color __debugColor = Color.red;

	    RaycastHit __hit;

	    Ray __mouseRay = _view.ScreenPointToRay(Input.mousePosition);

	    if (Physics.Raycast(__mouseRay, out __hit))
	    {
	        __debugColor = Color.green;

	        if (Input.GetKeyDown(KeyCode.Mouse1))
	        {
	            HandleMapObjectInteraction(__hit);
	        }
	    }

        Debug.DrawRay(_view.transform.position, __mouseRay.direction * 50, __debugColor, 1f);
	}

    private void HandleMapObjectInteraction(RaycastHit p_hit)
    {   
        MapObject __hitObject = p_hit.collider.GetComponent<MapObject>();

        if ((IActor)__hitObject != null)
        {
            BuildContextMenu(__hitObject);
            //__hitObject.Interact();
        }
    }

    private void BuildContextMenu(MapObject p_obect)
    {
        foreach (GameObject __child in _menuPanel.GetComponentsInChildren<RectTransform>().Select(c => c.gameObject).Except(new [] {_menuPanel.gameObject}))
        {
            Destroy(__child);
        }

        _menuPanel.DetachChildren();

        Vector2 __point;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_menuCanvas, Input.mousePosition, _view, out __point);

        _menuPanel.position = _menuCanvas.TransformPoint(__point);

        IActor __actions = p_obect as IActor;

        Vector2 __buttonSize = _menuButton.GetComponent<RectTransform>().sizeDelta;

        _menuPanel.sizeDelta = new Vector2(_menuPanel.sizeDelta.x, __buttonSize.y * __actions.ActionsList.Count);

        __actions.ActionsList.Select(a =>
        {
            RectTransform __actionButton = Instantiate(_menuButton).GetComponent<RectTransform>();

            ButtonSettings __settings = __actionButton.GetComponent<ButtonSettings>();

            __settings.Text = a.GetDescription();
            __settings.ClickAction = () =>
            {
                p_obect.Interact();
                _menuPanel.gameObject.SetActive(false);
            };

            return __actionButton;
        }).ToList().ForEach(b =>
        {
            b.SetParent(_menuPanel);
            b.localScale = Vector3.one;
            b.localRotation =  Quaternion.identity;
            b.localPosition = Vector3.zero;

            b.offsetMax = Vector2.down * 12 * (_menuPanel.childCount - 1);
            b.offsetMin = Vector2.down * 12 * _menuPanel.childCount;
        });

        _menuPanel.gameObject.SetActive(true);
    }
}
