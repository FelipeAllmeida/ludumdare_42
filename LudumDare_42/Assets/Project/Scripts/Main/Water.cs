﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class Water : MonoBehaviour
    {
        [SerializeField] public GameObject _underwaterPivot;
        public void SetActive(bool p_isActive)
        {
            gameObject.SetActive(p_isActive);
            _underwaterPivot.SetActive(p_isActive);
        }

        public void SetFillAmount(float p_fillAmount)
        {
            transform.localPosition = new Vector3(0f, (GameSettings.WATER_MAX_HEIGHT - GameSettings.WATER_MIN_HEIGHT) * p_fillAmount + GameSettings.WATER_MIN_HEIGHT, 0f);
            _underwaterPivot.transform.localScale = new Vector3(GameSettings.GROUND_SIZE, (GameSettings.WATER_MAX_HEIGHT - GameSettings.WATER_MIN_HEIGHT) * p_fillAmount, GameSettings.GROUND_SIZE);
        }
    }
}
