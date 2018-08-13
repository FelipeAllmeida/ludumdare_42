using Main.Game;
using Main.Menu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vox;
using UnityEngine;

namespace Main
{
    public enum EnvironmentControllers
    {
        None,
        Game,
        Menu
    }

    public class MainEnvironment : MonoBehaviour
    {
        [SerializeField] private EnvironmentControllers CurrentController;
        [SerializeField] public List<EnvironmentController> _listReferenceEnvironmentController;


        private Dictionary<EnvironmentControllers, EnvironmentController> _dictEnvironmentController = new Dictionary<EnvironmentControllers, EnvironmentController>();

        // Use this for initialization
        void Start()
        {
            InitializeControllers();
            Timer.WaitSeconds(.5f, () => ChangeController(CurrentController));
        }

        // Update is called once per frame
        void Update()
        {
            if (_dictEnvironmentController.ContainsKey(CurrentController))
            {
                _dictEnvironmentController[CurrentController].UpdateController();
            }
        }

        private void InitializeControllers()
        {
            _listReferenceEnvironmentController.ForEach(x =>
            {
                x.DisableController();
                ListenControllerEvents(x);
                _dictEnvironmentController.Add(x.Type, x);
            });
        }

        private void ListenControllerEvents(EnvironmentController p_controller)
        {
            p_controller.onRequestChangeController -= Controller_OnRequestChangeController;
            p_controller.onRequestChangeController += Controller_OnRequestChangeController;
        }

        private void Controller_OnRequestChangeController(object p_source, EnvironmentControllers p_nextController)
        {
            ChangeController(p_nextController);
        }

        private void ChangeController(EnvironmentControllers p_newController)
        {
            _dictEnvironmentController[CurrentController].DisableController();
            _dictEnvironmentController[p_newController].IntializeController();
            _dictEnvironmentController[p_newController].EnableController();

            CurrentController = p_newController;
        }
    }
}
