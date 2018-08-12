using Main.Game;
using Main.Menu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            ChangeController(CurrentController);
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
                _dictEnvironmentController.Add(x.Controller, x);
            });
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
