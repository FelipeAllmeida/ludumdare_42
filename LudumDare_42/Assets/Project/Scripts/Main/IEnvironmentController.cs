using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public interface IEnvironmentController
    {
        void IntializeController();
        void EnableController();
        void DisableController();
        void UpdateController();
    }

    public abstract class EnvironmentController : MonoBehaviour, IEnvironmentController
    {
        public event EventHandler<EnvironmentControllers> onRequestChangeController;

        [SerializeField] private EnvironmentControllers _controller;
        public EnvironmentControllers Type { get { return _controller; } }

        public bool IsInitialized { get; internal set; } = false;

        public virtual void DisableController()
        {
            IsInitialized = false;
            gameObject.SetActive(false);
        }

        protected void ChangeController(EnvironmentControllers p_nextController)
        {
            onRequestChangeController?.Invoke(this, p_nextController);
        }

        public virtual void IntializeController()
        {
            IsInitialized = true;
        }

        public virtual void EnableController()
        {
            gameObject.SetActive(true);
        }

        public virtual void UpdateController()
        {

        }
    }
}
