using Internal.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Menu
{
    public enum Views
    {
        Main,
        Credits,
        Tutorial
    }

    public class MenuController : EnvironmentController
    {
        [SerializeField] public List<View> _listReferenceViews;

        private Dictionary<Views, View> _dictViews = new Dictionary<Views, View>();

        private Views _currentView;

        public override void IntializeController()
        {
            base.IntializeController();

            if (_dictViews.Count == 0)
            {
                _listReferenceViews.ForEach(x => _dictViews.Add(x.Type, x));
            }

            foreach(View view in _dictViews.Values)
            {
                view.ForceHide();
                view.Initialize();

                if (view.Type == Views.Main)
                {
                    (view as MainView).onClickButtonStart -= MainView_OnClickButtonStart;
                    (view as MainView).onClickButtonStart += MainView_OnClickButtonStart;
                }

                view.onRequestChangeView = (p_source, p_nextView) =>
                {
                    ChangeView(p_nextView);
                };
            }
        }

        private void MainView_OnClickButtonStart()
        {
            ChangeController(EnvironmentControllers.Game);
        }

        public override void EnableController()
        {
            base.EnableController();
            AudioController.Instance.Play(Tags.Background_Menu);
            ChangeView(Views.Main);
        }

        public override void DisableController()
        {
            if (_dictViews.ContainsKey(_currentView))
            {
                _dictViews[_currentView].Hide(() =>
                {
                    AudioController.Instance.Stop(Tags.Background_Menu);
                    base.DisableController();
                });
            }
            else
            {
                AudioController.Instance.Stop(Tags.Background_Menu);
                base.DisableController();
            }
        }

        private void ChangeView(Views p_nextView)
        {
            Action __showNextView = () =>
            {
                _currentView = p_nextView;
                _dictViews[_currentView].Show(null);
            };

            if (_dictViews[_currentView].State == View.ViewStates.Showing)
            {
                _dictViews[_currentView].Hide(__showNextView);
            }
            else
            {
                __showNextView();
            }
        }
    }

}
